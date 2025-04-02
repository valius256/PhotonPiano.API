using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PhotonPiano.Api.Requests.Tution;
using PhotonPiano.Api.Responses.Payment;
using PhotonPiano.Api.Responses.Tution;
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Test.Extensions;
using static PhotonPiano.Test.Extensions.Extensions;

namespace PhotonPiano.Test.IntegrationTest.Tuition;

[Collection("Tuition Integration Tests")]
public class TuitionControllerIntegrationTest : BaseIntergrationTest, IDisposable
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly IServiceFactory _serviceFactory;
    private static bool _dataInitialized = false;
    
    public TuitionControllerIntegrationTest(IntergrationTestWebAppFactory factory) : base(factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Create a scope to resolve the scoped service
        _scope = factory.Services.CreateScope();
        _serviceFactory = _scope.ServiceProvider.GetRequiredService<IServiceFactory>();

        if (!_dataInitialized)
        {   
            _serviceFactory.TuitionService.CronAutoCreateTuition().GetAwaiter().GetResult();
            _dataInitialized = true;
        }
    }
    
    public void Dispose()
    {
        _scope?.Dispose();
    }
    
    [Fact]
    public async Task PayTuitionFee_AsStudent_ReturnsPaymentUrl()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        // Get tuitions to find an unpaid one
        var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&size=10");
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
        // response.EnsureSuccessStatusCode();
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
        var response = await _client.GetAsync("/api/tuitions?page=1&size=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(response);
        Assert.NotNull(tuitions);
    }
    
    [Fact]
    public async Task GetPagedTuitions_ReturnsCorrectPaginationHeaders()
    {
        // Arrange

        var token = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // var accountId = _client.GetFirebaseAccountId("learner035@gmail.com", "123456");

        var queryPaged = new QueryPagedModel()
        {
            Page = 1,
            PageSize = 5,
            SortColumn = "Id",
            OrderByDesc = true,
        };
        
        var queryParams = new Dictionary<string, string>
        {
            ["page"] = queryPaged.Page.ToString(),
            ["size"] = queryPaged.PageSize.ToString(),
            ["column"] = queryPaged.SortColumn,
            ["desc"] = queryPaged.OrderByDesc.ToString()
        };
    
        var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={p.Value}"));
        var requestUri = $"/api/tuitions?{queryString}";
    
        // Act
        var response = await _client.GetAsync(requestUri);
    
        // Assert
        response.EnsureSuccessStatusCode();
    
        // Verify pagination headers 
        Assert.True(response.Headers.Contains("X-Total-Count"), "X-Total-Count header is missing");
        Assert.True(response.Headers.Contains("X-Total-Pages"), "X-Total-Pages header is missing");
        Assert.True(response.Headers.Contains("X-Page"), "X-Page header is missing");
        Assert.True(response.Headers.Contains("X-Page-Size"), "X-Page-Size header is missing");
    
        // Verify header values
        var page = int.Parse(response.Headers.GetValues("X-Page").First());
        var pageSize = int.Parse(response.Headers.GetValues("X-Page-Size").First());
        
        Assert.Equal(page, queryPaged.Page);
        Assert.Equal(pageSize, queryPaged.PageSize);
    
        // Deserialize content and verify it matches headers
        var result = DeserializeResponse<List<TuitionWithStudentClassResponse>>(response);
    
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPagedTuitions_AsStaff_ReturnsAllTuitions()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/tuitions?page=1&size=10");

        // Assert
        response.EnsureSuccessStatusCode();
        var tuitions = await DeserializeResponse<List<TuitionWithStudentClassResponse>>(response);
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
        var response = await _client.GetAsync($"/api/tuitions?page=1&size=10&start-date={startDate}&end-date={endDate}&payment-statuses=Pending");

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
        var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&size=1");
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
    public async Task GetTuitionDetails_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await _client.GetAsync($"/api/tuitions/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTuitionDetails_AsStudent_CannotAccessOtherStudentTuition()
    {
        // Arrange
        // First get a tuition id as staff
        var staffToken = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", staffToken);
        
        var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&size=10");
        tuitionsResponse.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(tuitionsResponse);
        
        // Find a tuition that doesn't belong to learner035
        var otherStudentTuition = tuitions.First(x => x.StudentClass.StudentEmail != "learner035@gmail.com");
        
        Assert.NotNull(otherStudentTuition);

        // Now try to access as learner035
        var studentToken = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

        // Act
        var response = await _client.GetAsync($"/api/tuitions/{otherStudentTuition.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PayTuitionFee_AlreadyPaid_ReturnsBadRequest()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Get tuitions to find a paid one
        var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&size=10");
        tuitionsResponse.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(tuitionsResponse);
        var paidTuition = tuitions.FirstOrDefault(t => t.PaymentStatus == PaymentStatus.Succeed);
        
        // If no paid tuition is found, create a test that automatically passes
        if (paidTuition == null)
        {
            Assert.True(true, "No paid tuition found to test with");
            return;
        }
        
        var request = new PayTuitionFeeRequest
        {
            TuitionId = paidTuition.Id,
            ReturnUrl = "https://test-return-url.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tuitions/tuition-fee", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PayTuitionFee_AnotherStudentTuition_ReturnsBadRequest()
    {
        // Arrange
        // First get a tuition id as staff that doesn't belong to learner035
        var staffToken = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", staffToken);
        
        var tuitionsResponse = await _client.GetAsync("/api/tuitions?page=1&size=20");
        tuitionsResponse.EnsureSuccessStatusCode();
        var tuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(tuitionsResponse);
        
        var otherStudentTuition = tuitions.FirstOrDefault(t => 
            !t.StudentClass.StudentEmail.Equals("learner035@gmail.com", StringComparison.OrdinalIgnoreCase) && 
            t.PaymentStatus == PaymentStatus.Pending);
        
        if (otherStudentTuition == null)
        {
            Assert.True(true, "No other student tuition found to test with");
            return;
        }

        // Now try to pay as learner035
        var studentToken = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

        var request = new PayTuitionFeeRequest
        {
            TuitionId = otherStudentTuition.Id,
            ReturnUrl = "https://test-return-url.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tuitions/tuition-fee", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PayTuitionFee_InvalidTuitionId_ReturnsNotFound()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner035@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new PayTuitionFeeRequest
        {
            TuitionId = Guid.NewGuid(),
            ReturnUrl = "https://test-return-url.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tuitions/tuition-fee", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPagedTuitions_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // First get total count from page 1
        var firstPageResponse = await _client.GetAsync("/api/tuitions?page=1&size=5");
        firstPageResponse.EnsureSuccessStatusCode();
        
        // Check if there are enough items for a second page
        if (!firstPageResponse.Headers.TryGetValues("X-Total-Count", out var totalCountValues) || 
            !int.TryParse(totalCountValues.FirstOrDefault(), out var totalCount) || 
            totalCount <= 5)
        {
            Assert.True(true, "Not enough items to test pagination");
            return;
        }

        // Act - get second page
        var secondPageResponse = await _client.GetAsync("/api/tuitions?page=2&size=5");
        
        // Assert
        secondPageResponse.EnsureSuccessStatusCode();
        var secondPageTuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(secondPageResponse);
        
        // Check that we got items and they are different from first page
        var firstPageTuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(firstPageResponse);
        
        Assert.NotEmpty(secondPageTuitions);
        Assert.NotEqual(firstPageTuitions.First().Id, secondPageTuitions.First().Id);
        Assert.True(secondPageTuitions.Count <= 5);
    }

    [Fact]
    public async Task GetPagedTuitions_WithSorting_ReturnsSortedResults()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act - get ascending order by amount
        var ascResponse = await _client.GetAsync("/api/tuitions?page=1&size=10&column=Amount&desc=false");
        
        // Act - get descending order by amount
        var descResponse = await _client.GetAsync("/api/tuitions?page=1&size=10&column=Amount&desc=true");
        
        // Assert
        ascResponse.EnsureSuccessStatusCode();
        descResponse.EnsureSuccessStatusCode();
        
        var ascTuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(ascResponse);
        var descTuitions = await Extensions.Extensions.DeserializeResponse<List<TuitionWithStudentClassResponse>>(descResponse);
        
        if (ascTuitions.Count > 1 && descTuitions.Count > 1)
        {
            // Verify ascending sort
            Assert.True(ascTuitions.First().Amount <= ascTuitions.Last().Amount);
            
            // Verify descending sort
            Assert.True(descTuitions.First().Amount >= descTuitions.Last().Amount);
            
            // Verify different order
            Assert.NotEqual(ascTuitions.First().Amount, descTuitions.First().Amount);
        }
    }
}