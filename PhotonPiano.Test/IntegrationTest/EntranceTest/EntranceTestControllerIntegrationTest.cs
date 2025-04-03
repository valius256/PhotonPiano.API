using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Responses.Payment;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Test.Extensions;

namespace PhotonPiano.Test.IntegrationTest.EntranceTest;

public class EntranceTestControllerIntegrationTest : BaseIntergrationTest
{
    private readonly HttpClient _client;

    private readonly string _baseUrl = "/api/entrance-tests";
    
    public EntranceTestControllerIntegrationTest(IntergrationTestWebAppFactory factory) : base(factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetEntranceTests_Unauthorized_ReturnsUnauthorized()
    {
        //Act
        var response = await _client.GetAsync($"{_baseUrl}");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEntranceTests_GetSuccessAndAuthorized_ReturnsOkStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var queryRequest = new QueryEntranceTestRequest
        {
            Page = 1,
            PageSize = 5,
            SortColumn = "CreatedAt",
            OrderByDesc = false
        };
        
        string url = $"{_baseUrl}?page={queryRequest.Page}&size={queryRequest.PageSize}&column={queryRequest.SortColumn}&desc={queryRequest.OrderByDesc}";
        
        //Act
        var response = await _client.GetAsync(url);

        var headers = response.Headers;
        
        var page = headers.GetValues("X-Page").FirstOrDefault();
        var pageSize = headers.GetValues("X-Page-Size").FirstOrDefault();
        var totalCount = headers.GetValues("X-Total-Count").FirstOrDefault();
        var totalPages = headers.GetValues("X-Total-Pages").FirstOrDefault();
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(page, page);
        Assert.Equal(pageSize, pageSize);
        Assert.NotNull(totalCount);
        Assert.NotNull(totalPages);
    }
    
    [Fact]
    public async Task GetEntranceTestDetails_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        
        //Act 
        var response = await _client.GetAsync($"{_baseUrl}/682e3e48-50ca-464e-aea5-1964e3a03811");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEntranceTestDetails_EntranceTestNotFound_ReturnsNotFoundStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var entranceTestId = Guid.NewGuid();

        //Act 
        var response = await _client.GetAsync($"{_baseUrl}/{entranceTestId}");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetEntranceTestDetails_GetSuccessAndAuthorized_ReturnsOkStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        Guid entranceTestId = Guid.Parse("3c707d90-f81f-4f35-a089-829c8d36bbe2");
        
        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{entranceTestId}");
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetEntranceTestStudentDetails_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var id = Guid.NewGuid();
        var studentId = Guid.NewGuid().ToString();
        
        string url = $"{_baseUrl}/{id}/students/{studentId}";
        
        //Act
        var response = await _client.GetAsync(url);
        
        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEntranceTestStudentDetails_EntranceTestStudentNotFound_ReturnsNotFoundStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var entranceTestId = Guid.NewGuid();
        var studentId = Guid.NewGuid().ToString();

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{entranceTestId}/students/{studentId}");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetEntranceTestStudentDetails_GetSuccessAndAuthorized_ReturnsOkStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var entranceTestId = "682e3e48-50ca-464e-aea5-1964e3a03811";
        var studentId = "GVX3Q2a70aU3OansV69wbTtvddY2";

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{entranceTestId}/students/{studentId}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateEntranceTest_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var createRequest = new CreateEntranceTestRequest
        {
            Name = "Test Entrance Test",
            RoomId = Guid.NewGuid(),
        };
        
        string json = JsonConvert.SerializeObject(createRequest);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PostAsync($"{_baseUrl}", content);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateEntranceTest_CreateSuccess_ReturnsCreatedStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateEntranceTestRequest
        {
            Name = "Test Entrance Test",
            RoomId = Guid.Parse("097eaa43-c599-4eae-a52d-f86733f28cd0"),
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            Shift = Shift.Shift3_10h45_12h,
            InstructorId = "1axRN4fG0ybZyDOYvO8wnkm5lHJ3",
            IsAnnouncedScore = false
        };
        
        string json = JsonConvert.SerializeObject(createRequest);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PostAsync($"{_baseUrl}", content);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task EnrollEntranceTest_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        string returnUrl = "https://www.youtube.com/";
        var request = new EnrollmentRequest(returnUrl);
        
        string json = JsonConvert.SerializeObject(request);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        
        //Act
        var response = await _client.PostAsync($"{_baseUrl}/enrollment-requests", content);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task EnrollEntranceTest_EnrollSuccess_ReturnsCreatedStatusWithPaymentUrl()
    {
        //Arrange
        string returnUrl = "https://www.youtube.com/";
        var request = new EnrollmentRequest(returnUrl);
                
        var token = await _client.GetAuthToken("learner041@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        string json = JsonConvert.SerializeObject(request);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        
        //Act
        var response = await _client.PostAsync($"{_baseUrl}/enrollment-requests", content);
        
        var responseJsonString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<PaymentUrlResponse>(responseJsonString);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(responseObject);
        Assert.NotEmpty(responseObject.Url);
    }

    [Fact]
    public async Task DeleteEntranceTest_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{Guid.NewGuid()}");
        
        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEntranceTest_AuthorizedAndEntranceTestNotFound_ReturnsNotFoundStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var entranceTestId = Guid.NewGuid();
        
        //Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{entranceTestId}");
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteEntranceTest_AuthorizedAndDeleteSuccess_ReturnsNoContentStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var entranceTestId = "682e3e48-50ca-464e-aea5-1964e3a03811";
        
        //Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{entranceTestId}");
        
        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}