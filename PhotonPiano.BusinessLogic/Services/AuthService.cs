using System.Security.Cryptography;
using System.Text;
using Mapster;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly string _apiKey;

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork, IServiceFactory serviceFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
        _configuration = configuration;
        _apiKey = _configuration["Firebase:Auth:ApiKey"]!;
    }

    public async Task<AuthModel> SignIn(string email, string password)
    {
        var account =
            await _unitOfWork.AccountRepository.FindSingleAsync(a =>
                a.Email == email && HashPassword(password) == a.Password);

        if (account is null) throw new UnauthorizedException("Wrong email or password");

        if (account.Status == AccountStatus.Inactive) throw new ForbiddenMethodException("Account is inactive");

        var idToken = _serviceFactory.TokenService.GenerateIdToken(account.Adapt<AccountModel>());

        var refreshToken = _serviceFactory.TokenService.GenerateRefreshToken();

        account.RefreshToken = refreshToken;
        account.RefreshTokenExpiryDate = DateTime.UtcNow.AddHours(7).AddDays(30);

        await _unitOfWork.SaveChangesAsync();

        return new AuthModel
        {
            Kind = account.Email,
            LocalId = account.AccountFirebaseId,
            Email = account.Email,
            DisplayName = account.FullName ?? account.Email,
            ExpiresIn = "3600",
            IdToken = idToken,
            RefreshToken = account.RefreshToken,
            Role = account.Role,
            Registered = true
        };
    }

    public async Task<AuthModel> SignUp(SignUpModel model)
    {
        var (email, password) = model;

        if (await _unitOfWork.AccountRepository.AnyAsync(a => a.Email == email))
        {
            throw new ConflictException("Email already exists");
        }

        var account = model.Adapt<Account>();

        account.AccountFirebaseId = Guid.NewGuid().ToString();
        account.Password = HashPassword(password);
        account.Role = Role.Student;
        account.StudentStatus = StudentStatus.Unregistered;

        var idToken = _serviceFactory.TokenService.GenerateIdToken(account.Adapt<AccountModel>());

        var refreshToken = _serviceFactory.TokenService.GenerateRefreshToken();

        account.RefreshToken = refreshToken;
        account.RefreshTokenExpiryDate = DateTime.UtcNow.AddHours(7).AddDays(30);

        bool requiresEntranceTestRegistering = true;

        //Skip entrance test if self evaluated level doesn't requires entrance test participation
        if (account.SelfEvaluatedLevelId.HasValue)
        {
            var selfEvaluatedLevel =
                await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == account.SelfEvaluatedLevelId.Value,
                    hasTrackings: false);

            if (selfEvaluatedLevel?.RequiresEntranceTest == false)
            {
                account.StudentStatus = StudentStatus.WaitingForClass;
                account.LevelId = account.SelfEvaluatedLevelId.Value;
                await _serviceFactory.NotificationService.SendNotificationsToAllStaffsAsync(
                    $"Learner {account.FullName} is waiting for {selfEvaluatedLevel.Name.ToUpper()} level class arrangement",
                    "", requiresSavingChanges: false);
                requiresEntranceTestRegistering = false;
            }
        }

        await _unitOfWork.AccountRepository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return new AuthModel
        {
            Kind = account.Email,
            LocalId = account.AccountFirebaseId,
            Email = account.Email,
            DisplayName = account.FullName ?? account.Email,
            ExpiresIn = "3600",
            IdToken = idToken,
            RefreshToken = refreshToken,
            Role = account.Role,
            Registered = true,
            RequiresEntranceTestRegistering = requiresEntranceTestRegistering
        };
    }

    public async Task<NewIdTokenModel> RefreshToken(string refreshToken)
    {
        var account = await _unitOfWork.AccountRepository.FindFirstAsync(a => a.RefreshToken == refreshToken);

        if (account is null) throw new UnauthorizedException("Wrong refresh token");

        if (account.RefreshToken == string.Empty || account.RefreshTokenExpiryDate < DateTime.UtcNow.AddHours(7))
            throw new BadRequestException("Invalid refresh token or refresh token is expired");

        return new NewIdTokenModel
        {
            Role = account.Role.ToString(),
            ExpiresIn = "3600",
            IdToken = _serviceFactory.TokenService.GenerateIdToken(account.Adapt<AccountModel>()),
            ProjectId = "PhotonPiano",
            RefreshToken = account.RefreshToken,
            TokenType = "idToken",
            UserId = account.AccountFirebaseId
        };
    }

    public async Task SendPasswordResetEmail(string email)
    {
        var account = await _unitOfWork.AccountRepository.FindFirstAsync(a => a.Email == email);
        if (account is null) throw new NotFoundException("Account not found");

        var resetUrl = _configuration["PasswordResetBaseUrl"];
        if (string.IsNullOrWhiteSpace(resetUrl)) throw new Exception("Reset Url not found!");

        account.ResetPasswordToken = AuthUtils.GenerateSecureToken();
        account.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(7).AddHours(1);
        await _unitOfWork.AccountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();

        var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "resetPassswordToken", $"{account.ResetPasswordToken}" },
            { "email", $"{account.Email}" },
            { "url", $"{resetUrl}" },
            { "Subject", "[PhotonPiano] Yêu cầu đặt lại mật khẩu" }
        };
        await _serviceFactory.EmailService.SendAsync("ResetPassword", [email], [], emailParam);
    }

    public async Task<OAuthCredentialsModel> HandleGoogleAuthCallback(string code, string redirectUrl)
    {
        using var client = _httpClientFactory.CreateClient();

        var snakeCaseJsonSerializerSetting = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        var url = "https://oauth2.googleapis.com/token";

        var request = JsonConvert.SerializeObject(new
        {
            code,
            client_id = _configuration["Google:ClientId"],
            client_secret = _configuration["Google:ClientSecret"],
            redirect_uri = redirectUrl,
            grant_type = "authorization_code"
        }, snakeCaseJsonSerializerSetting);

        var content = new StringContent(request, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        // _logger.LogInformation(responseContent);
        //
        // _logger.LogInformation("Status code: " + response.StatusCode.ToString());

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            throw new BadRequestException("Invalid authorized code");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseObject =
            JsonConvert.DeserializeObject<GoogleAuthModel>(jsonResponse, snakeCaseJsonSerializerSetting)!;

        var idToken = responseObject.IdToken;

        var credentials = await LinkWithFirebaseOAuthCredentials(code, idToken);

        return credentials;
    }

    public async Task UpdateFirebaseEmail(string idToken, string newEmail)
    {
        using var client = _httpClientFactory.CreateClient();

        var url =
            $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={_configuration["Firebase:Auth:ApiKey"]}";

        var jsonRequest = JsonConvert.SerializeObject(new
        {
            idToken,
            email = newEmail,
            returnSecureToken = true
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(errorResponse);
            var errorResponseObject = JsonConvert.DeserializeObject<FirebaseErrorResponseModel>(errorResponse)!;
            throw new BadRequestException(errorResponseObject.Message);
        }
    }

    public async Task<string> SignUpOnFirebase(string email, string password)
    {
        using var client = _httpClientFactory.CreateClient();

        var userId = string.Empty;

        var jsonRequest = JsonConvert.SerializeObject(new
        {
            email,
            password,
            returnSecureToken = true
        });

        var url =
            $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={_apiKey}";

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var errorResponseObject = JsonConvert.DeserializeObject<FirebaseErrorResponseModel>(errorResponse)!;
            throw new CustomException(errorResponseObject.Message, errorResponseObject.Code);
        }

        // Read the response content and parse it
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        // Extract the localId field from the response
        userId = responseObject is not null ? responseObject.localId : string.Empty;
        return userId;
    }

    public async Task ChangePassword(ChangePasswordModel changePasswordModel)
    {
        var account = await _unitOfWork.AccountRepository.FindFirstAsync(a =>
            a.ResetPasswordToken == changePasswordModel.ResetPasswordToken && a.Email == changePasswordModel.Email);
        if (account is null) throw new ForbiddenMethodException("Invalid token or email");

        if (account.ResetPasswordToken == null || account.ResetPasswordTokenExpiry < DateTime.UtcNow.AddHours(7))
            throw new BadRequestException("Token expired or does not exist");

        account.Password = AuthUtils.HashPassword(changePasswordModel.Password);
        account.ResetPasswordToken = null;
        account.ResetPasswordTokenExpiry = null;
        await _unitOfWork.AccountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ToggleAccountStatus(string firebaseUid)
    {
        var currentUser = await _unitOfWork.AccountRepository.FindFirstAsync(x => x.AccountFirebaseId == firebaseUid);

        if (currentUser is null) throw new NotFoundException("Account not found");

        if (currentUser.Status == AccountStatus.Active)
        {
            currentUser.Status = AccountStatus.Inactive;
            currentUser.RefreshToken = string.Empty;
            currentUser.RefreshTokenExpiryDate = DateTime.UtcNow.AddHours(7);
        }
        else
        {
            currentUser.Status = AccountStatus.Active;
        }

        await _unitOfWork.AccountRepository.UpdateAsync(currentUser);
        await _unitOfWork.SaveChangesAsync();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        // Convert the password string to bytes
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        // Compute the hash
        var hashBytes = sha256.ComputeHash(passwordBytes);

        // Convert the hash to a hexadecimal string
        var hashedPassword = string.Concat(hashBytes.Select(b => $"{b:x2}"));

        return hashedPassword;
    }

    private bool VerifyPassword(string inputPassword, string dbHashedPassword)
    {
        var hashedPassword = HashPassword(inputPassword);

        return hashedPassword == dbHashedPassword;
    }

    private async Task<OAuthCredentialsModel> LinkWithFirebaseOAuthCredentials(string googleCode, string idToken)
    {
        using var client = _httpClientFactory.CreateClient();

        var requestUri =
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={_configuration["Firebase:Auth:ApiKey"]}";

        var requestData =
            new StringContent(
                $"{{\"postBody\":\"id_token={idToken}&providerId=google.com\",\"returnSecureToken\":true,\"requestUri\":\"http://localhost:7168/scalar/v1\"}}",
                Encoding.UTF8, "application/json");
        var response = await client.PostAsync(requestUri, requestData);

        if (!response.IsSuccessStatusCode) throw new UnauthorizedException("Unauthorized");

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var responseObject = JsonConvert.DeserializeObject<OAuthCredentialsModel>(jsonResponse)!;

        var (firebaseId, emailVerified, email, firebaseIdToken, refreshToken, expiresIn, _) = responseObject;

        var account = await _serviceFactory.AccountService.GetAndCreateAccountIfNotExistsCredentials(firebaseId, email,
            requireCheckingFirebaseId: false);

        responseObject.Role = account.Role.ToString();

        return responseObject;
    }
}