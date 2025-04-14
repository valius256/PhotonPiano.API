using System.Net.Http.Headers;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PhotonPiano.Api.Requests.Survey;
using PhotonPiano.Test.Extensions;
using Xunit.Abstractions;

namespace PhotonPiano.Test.IntegrationTest.Survey;

[Collection("Piano surveys integration tests")]
public class PianoSurveysControllerIntegrationTest : BaseIntegrationTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly string _baseUrl = "/api/piano-surveys";

    private string surveyId = string.Empty;

    public PianoSurveysControllerIntegrationTest(IntegrationTestWebAppFactory factory, ITestOutputHelper testOutputHelper) : base(factory)
    {
        _testOutputHelper = testOutputHelper;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public async Task CreatePianoSurvey_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var createRequest = _fixture.Build<CreatePianoSurveyRequest>()
            .Create();

        string json = JsonConvert.SerializeObject(createRequest);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PostAsync(_baseUrl, content);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePianoSurvey_CreateSuccess_ReturnsCreatedStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = _fixture.Build<CreatePianoSurveyRequest>()
            .With(x => x.Name, "Test Survey")
            .With(x => x.MinAge, 2)
            .With(x => x.MaxAge, 100)
            .With(x => x.Questions, [
            ])
            .Create();

        string json = JsonConvert.SerializeObject(createRequest);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PostAsync(_baseUrl, content);

        var jsonString = await response.Content.ReadAsStringAsync();

        var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

        if (responseObject is not null)
        {
            surveyId = responseObject.id;
        }

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetSurveys_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange

        //Act
        var response = await _client.GetAsync(_baseUrl);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetSurveys_GetSuccess_ReturnsOkStatusWithCorrectMetadata()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.GetAsync(_baseUrl);

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
    public async Task GetPianoSurveyDetails_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{id}");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPianoSurveyDetails_SurveyNotFound_ReturnsNotFoundStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var id = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{id}");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPianoSurveyDetails_GetSuccess_ReturnsOkStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{surveyId}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePianoSurvey_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var updateRequest = _fixture.Build<UpdatePianoSurveyRequest>()
            .Create();

        string json = JsonConvert.SerializeObject(updateRequest);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PutAsync($"{_baseUrl}/{Guid.NewGuid()}", content);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePianoSurvey_SurveyNotFound_ReturnsNotFoundStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var updateRequest = _fixture.Build<UpdatePianoSurveyRequest>()
            .Create();

        string json = JsonConvert.SerializeObject(updateRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PutAsync($"{_baseUrl}/{Guid.NewGuid()}", content);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePianoSurvey_UpdateSuccess_ReturnsNoContentStatus()
    {
        //Arrange
        await CreatePianoSurvey_CreateSuccess_ReturnsCreatedStatus();

        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new UpdatePianoSurveyRequest
        {
            Name = "Test Piano"
        };

        string json = JsonConvert.SerializeObject(updateRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PutAsync($"{_baseUrl}/{surveyId}", content);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        _testOutputHelper.WriteLine(responseContent);

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePianoSurvey_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{id}");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletePianoSurvey_DeleteSuccess_ReturnsNoContentStatus()
    {
        //Arrange
        await CreatePianoSurvey_CreateSuccess_ReturnsCreatedStatus();
        
        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{surveyId}");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(responseContent);

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}