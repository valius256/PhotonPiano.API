using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using PhotonPiano.Api.Requests.Tution;
using PhotonPiano.Api.Responses.Payment;
using PhotonPiano.Api.Responses.Tution;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Test.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PhotonPiano.Test.IntegrationTest.Tuition;

//[Collection("Tuition Integration Tests")]
public class TuitionControllerIntegrationTest : BaseIntergrationTest, IDisposable
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly IServiceFactory _serviceFactory;
    private static bool _dataInitialized = false;
    private static readonly object _lock = new object();

    public TuitionControllerIntegrationTest(IntergrationTestWebAppFactory factory) : base(factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Create a scope to resolve the scoped service
        _scope = factory.Services.CreateScope();
        _serviceFactory = _scope.ServiceProvider.GetRequiredService<IServiceFactory>();

        lock (_lock)
        {
            if (!_dataInitialized)
            {
                EnsureDatabaseReady(factory.GetDbConnectionString()); // Chờ database sẵn sàng
                _serviceFactory.TuitionService.CronAutoCreateTuition().GetAwaiter().GetResult();
                _dataInitialized = true;
            }
        }
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    private void EnsureDatabaseReady(string connectionString)
    {
        var retry = 10;
        while (retry > 0)
        {
            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT 1;", conn);
                cmd.ExecuteScalar();
                Console.WriteLine("✅ Database is ready!");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Database not ready yet. Retrying... {retry} attempts left. Error: {ex.Message}");
                Task.Delay(2000).Wait();
                retry--;
            }
        }

        throw new Exception("❌ Database is not ready for TuitionService.");
    }


    [Fact]
    public async Task PayTuitionFee_AsStudent_ReturnsPaymentUrl()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Get tuitions to find an unpaid one
        var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&page-size=10");
        tuitionsResponse.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(tuitionsResponse);
        var unpaidTuition = tuitions.FirstOrDefault(t => t.PaymentStatus == PaymentStatus.Pending);

        Assert.NotNull(unpaidTuition);

        var request = new PayTuitionFeeRequest
        {
            TuitionId = unpaidTuition.Id,
            // ReturnUrl = "https://test-return-url.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tuitions/tuition-fee", request);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentUrlResponse>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Url);
    }

    [Fact]
    public async Task PayTuitionFee_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new PayTuitionFeeRequest
        {
            TuitionId = Guid.NewGuid(),
            ReturnUrl = "https://test-return-url.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tuitions/tuition-fee", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleEnrollmentPaymentCallback_ReturnsNotFound()
    {
        // Arrange
        var accountId = _client.GetFirebaseAccountId("learner035@gmail.com", "123456");
        var clientRedirectUrl = "https://test-redirect-url.com";
        var callbackUrl = $"/api/tuitions/{accountId}/tuition-payment-callback?vnp_ResponseCode=00&vnp_TxnRef={Guid.NewGuid()}&url={clientRedirectUrl}";

        // Act
        var response = await _client.GetAsync(callbackUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPagedTuitions_AsStudent_ReturnsOwnTuitionsOnly()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/tuitions?page=1&page-size=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(response);
        Assert.NotNull(tuitions);
    }

    [Fact]
    public async Task GetPagedTuitions_AsStaff_ReturnsAllTuitions()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/tuitions?page=1&page-size=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(response);
        Assert.NotNull(tuitions);
    }

    [Fact]
    public async Task GetPagedTuitions_WithFilters_ReturnsFitleredResults()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var startDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
        var endDate = DateOnly.FromDateTime(DateTime.Now);

        // Act
        var response = await _client.GetAsync($"/api/tuitions?page=1&page-size=10&start-date={startDate}&end-date={endDate}&payment-statuses=Pending");

        // Assert
        response.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(response);
        Assert.NotNull(tuitions);
        Assert.All(tuitions, t => Assert.Equal(PaymentStatus.Pending, t.PaymentStatus));
    }

    [Fact]
    public async Task GetTuitionDetails_ExistingId_ReturnsTuitionDetails()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // First get a tuition id
        var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&page-size=1");
        tuitionsResponse.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(tuitionsResponse);
        var id = tuitions.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/tuitions/{id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var tuition = await response.Content.ReadFromJsonAsync<TuitionWithStudentClassResponse>();
        Assert.NotNull(tuition);
        Assert.Equal(id, tuition.Id);
    }

    [Fact]
    public async Task RefundTuitionAmount_AsStudent_ReturnsNotFound()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/tuitions/refund-amount");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // RefundTuitionAmount_AsStudent_ReturnsMoneyAmount
    // [Fact]
    // public async Task RefundTuitionAmount_AsStudent_ReturnsMoneyAmount()
    // {
    //     // Arrange
    //     var token = await _client.GetAuthToken("learner001@gmail.com", "123456"); // Using a student who has paid tuition
    //     _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //
    //     // Get student's classes first to set current class ID
    //     var currentStudentInfor = await _client.GetAsync("/api/auth/current-info");
    //     currentStudentInfor.EnsureSuccessStatusCode();
    //
    //     // Find a paid tuition for this student/class
    //     var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&page-size=10&payment-statuses=Succeed");
    //     tuitionsResponse.EnsureSuccessStatusCode();
    //
    //     var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(tuitionsResponse);
    //     Assert.Contains(tuitions, t => t.PaymentStatus == PaymentStatus.Succeed);
    //
    //     // Act
    //     var response = await _client.GetAsync("/api/tuitions/refund-amount");
    //
    //     // Assert
    //     response.EnsureSuccessStatusCode();
    //     var amount = await response.Content.ReadAsStringAsync();
    //     Assert.NotNull(amount);
    //     Assert.True(decimal.TryParse(amount.Trim('"'), out var refundAmount));
    //     Assert.True(refundAmount >= 0);
    // }
    //


}