using System.Net.Http.Headers;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PhotonPiano.Api.Requests.SurveyQuestion;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Test.Extensions;

namespace PhotonPiano.Test.IntegrationTest.Survey;

[Collection("Survey questions integration tests")]
public class SurveyQuestionsControllerIntegrationTest : BaseIntegrationTest
{
    private readonly HttpClient _client;

    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private readonly string _baseUrl = "/api/survey-questions";

    private string questionId = string.Empty;

    public SurveyQuestionsControllerIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public async Task CreateSurveyQuestion_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var request = _fixture.Build<CreateSurveyQuestionRequest>()
            .Create();

        string json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PostAsync(_baseUrl, content);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateSurveyQuestion_CreateSuccess_ReturnsCreatedStatus()
    {
        //Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = _fixture.Build<CreateSurveyQuestionRequest>()
            .With(x => x.Type, QuestionType.OpenText)
            .With(x => x.Options, [])
            .With(x => x.OrderIndex, 0)
            .With(x => x.SurveyId, (Guid?)null)
            .With(x => x.MinAge, (int?)null)
            .With(x => x.MaxAge, (int?)null)
            .Create();

        string json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PostAsync(_baseUrl, content);

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseObject = JsonConvert.DeserializeObject<SurveyQuestionModel>(responseContent);

        if (responseObject is not null)
        {
            questionId = responseObject.Id.ToString();
        }

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetSurveyQuestions_GetSuccess_ReturnsOkStatusWithCorrectMetadata()
    {
        //Arrange

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
    public async Task GetSurveyQuestionDetails_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{id}");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetSurveyQuestionDetails_QuestionNotFound_ReturnsNotFoundStatus()
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
    public async Task GetSurveyQuestionDetails_GetSuccess_ReturnsOkStatus()
    {
        //Arrange

        await CreateSurveyQuestion_CreateSuccess_ReturnsCreatedStatus();

        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{questionId}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAnswersOfQuestion_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{id}/answers");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAnswersOfQuestion_GetSuccess_ReturnsOkStatus()
    {
        //Arrange
        await CreateSurveyQuestion_CreateSuccess_ReturnsCreatedStatus();

        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.GetAsync($"{_baseUrl}/{questionId}/answers");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSurveyQuestion_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var id = Guid.NewGuid();

        var request = new UpdateSurveyQuestionRequest
        {
            QuestionContent = "Test"
        };

        string json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //Act
        var response = await _client.PutAsync($"{_baseUrl}/{id}", content);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSurveyQuestion_UpdateSuccess_ReturnsNoContentStatus()
    {
        //Arrange
        await CreateSurveyQuestion_CreateSuccess_ReturnsCreatedStatus();

        var request = new UpdateSurveyQuestionRequest
        {
            QuestionContent = "Test"
        };

        string json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.PutAsync($"{_baseUrl}/{questionId}", content);

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSurveyQuestion_Unauthorized_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{id}");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSurveyQuestion_DeleteSuccess_ReturnsNoContentStatus()
    {
        //Arrange
        await CreateSurveyQuestion_CreateSuccess_ReturnsCreatedStatus();

        var token = await _client.GetAuthToken("staff123@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{questionId}");

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}