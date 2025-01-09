using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.Test.Extensions;
using Xunit.Abstractions;

namespace PhotonPiano.Test.IntegrationTest.EntranceTestStudent;

public class EntranceTestStudentIntegrationTest(BaseApiConfig fixture, ITestOutputHelper testOutputHelper) : IClassFixture<BaseApiConfig>
{
    private readonly HttpClient _client = fixture.CreateClient();

    #region Test Get EntranceTestStudents

    [Fact]
    public async Task GetEntranceTestStudents_ReturnsListEntranceTestStudents_ReturnValidEntranceTestStudents()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/entranceTest");
        var content = await response.Content.ReadAsStringAsync();
        var quizzes = JsonConvert.DeserializeObject<List<EntranceTestStudentDetail>>(content, new JsonSerializerSettings());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(quizzes);
        Assert.NotEmpty(quizzes);
    }


    #endregion


}