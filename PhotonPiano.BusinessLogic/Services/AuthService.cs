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
using System.Text;

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
        using var client = _httpClientFactory.CreateClient();

        var url =
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";

        var jsonRequest = JsonConvert.SerializeObject(new
        {
            email,
            password,
            returnSecureToken = true
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            throw new UnauthorizedException("Wrong email or password");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var responseObject = JsonConvert.DeserializeObject<AuthModel>(jsonResponse)!;

        var account = await _serviceFactory.AccountService.GetAndCreateAccountIfNotExistsCredentials(
            responseObject.LocalId,
            email);

        responseObject.Role = account.Role;

        // var notifications = await _serviceFactory.NotificationService.GetUserNotificationsAsync(responseObject.LocalId);
        //
        // foreach (var notification in notifications)
        // {
        //     // Split Content back into Title and Message
        //     var splitContent = notification.Notification.Content.Split(new[] { ": " }, 2, StringSplitOptions.None);
        //     var title = splitContent.Length > 1 ? splitContent[0] : "Notification";
        //     var message = splitContent.Length > 1 ? splitContent[1] : notification.Notification.Content;
        //
        //     await _serviceFactory.NotificationServiceHub.SendNotificationAsync(
        //         responseObject.LocalId,
        //         account.UserName,
        //         title,
        //         message
        //     );
        // }

        return responseObject;
    }

    public async Task<AccountModel> SignUp(SignUpModel model)
    {
        var (email, password) = model;

        if (await _unitOfWork.AccountRepository.AnyAsync(a => a.Email == email))
            throw new ConflictException("Email already exists");

        var firebaseId = await SignUpOnFirebase(email, password);

        var account = model.Adapt<Account>();

        account.AccountFirebaseId = firebaseId;
        account.Role = Role.Student;
        account.StudentStatus = StudentStatus.Unregistered;

        await _unitOfWork.AccountRepository.AddAsync(account);

        await _unitOfWork.SaveChangesAsync();

        return account.Adapt<AccountModel>();
    }

    public async Task<NewIdTokenModel> RefreshToken(string refreshToken)
    {
        using var client = _httpClientFactory.CreateClient();

        var url = $"https://securetoken.googleapis.com/v1/token?key={_configuration["Firebase:Auth:ApiKey"]}";

        var snakeCaseJsonSerializerSetting = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        var jsonRequest = JsonConvert.SerializeObject(new
        {
            grant_type = "refresh_token",
            refresh_token = refreshToken
        }, snakeCaseJsonSerializerSetting);

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var errorResponseObject = JsonConvert.DeserializeObject<FirebaseErrorResponseModel>(errorResponse)!;
            throw new BadRequestException(errorResponseObject.Message);
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseObject =
            JsonConvert.DeserializeObject<NewIdTokenModel>(jsonResponse, snakeCaseJsonSerializerSetting)!;

        var account =
            await _unitOfWork.AccountRepository.FindFirstAsync(a => a.AccountFirebaseId == responseObject.UserId);

        responseObject.Role = account!.Role.ToString();

        return responseObject!;
    }

    public async Task SendPasswordResetEmail(string email)
    {
        using var client = _httpClientFactory.CreateClient();

        var url =
            $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={_configuration["Firebase:Auth:ApiKey"]}";

        var jsonRequest = JsonConvert.SerializeObject(new
        {
            requestType = "PASSWORD_RESET",
            email
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<FirebaseErrorResponseModel>(jsonResponse)!;

            throw new CustomException(responseObject.Message, responseObject.Code);
        }
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

        string url =
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
            Console.WriteLine(errorResponse.ToString());
            var errorResponseObject = JsonConvert.DeserializeObject<FirebaseErrorResponseModel>(errorResponse)!;
            throw new BadRequestException(errorResponseObject.Message);
        }
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

    private async Task<string> SignUpOnFirebase(string email, string password)
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
}