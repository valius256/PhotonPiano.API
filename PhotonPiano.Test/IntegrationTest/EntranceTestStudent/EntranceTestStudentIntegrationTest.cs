using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using PhotonPiano.Api.Requests.Auth;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared;
using PhotonPiano.Test.Extensions;
using Xunit.Abstractions;

namespace PhotonPiano.Test.IntegrationTest.EntranceTestStudent;

public class EntranceTestStudentIntegrationTest(BaseApiConfig fixture, ITestOutputHelper testOutputHelper)
    : IClassFixture<BaseApiConfig>
{
    private readonly HttpClient _client = fixture.CreateClient();

    #region Test Get EntranceTest

    [Fact]
    public async Task GetEntranceTests_ReturnsListEntranceTests_ReturnValidEntranceTest()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/entrance-tests");
        var content = await response.Content.ReadAsStringAsync();
        var entranceTests =
            JsonConvert.DeserializeObject<List<EntranceTestResponse>>(content, new JsonSerializerSettings());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(entranceTests);
        Assert.NotEmpty(entranceTests);
    }

    [Fact]
    public async Task GetEntranceTestStudents_WithValidFiltersWithValidRoomId_ReturnsExpectedEntranceTests()
    {
        // Arrange
        var roomResponse = await _client.GetAsync("/api/rooms");
        var rooms =
            JsonConvert.DeserializeObject<List<RoomDetailModel>>(await roomResponse.Content.ReadAsStringAsync());
        if (rooms == null) throw new Exception("No rooms found in database.");

        var query = new QueryEntranceTestRequest
        {
            RoomIds = new List<Guid> { rooms.First().Id },
            Keyword = "Room",
            IsAnnouncedScore = true
        };
        // Act

        var queryString = new QueryStringHelper().ToQueryString(query);
        var requestUrl = $"/api/entrance-tests{queryString}";

        var response = await _client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();
        var entranceTests =
            JsonConvert.DeserializeObject<List<EntranceTestResponse>>(content, new JsonSerializerSettings());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(entranceTests);
        Assert.NotEmpty(entranceTests);
    }

    [Fact]
    public async Task GetEntranceTestStudents_WithValidFiltersWithValidShift_ReturnsExpectedEntranceTests()
    {
        // Arrange
        var roomResponse = await _client.GetAsync("/api/rooms");
        var rooms =
            JsonConvert.DeserializeObject<List<RoomDetailModel>>(await roomResponse.Content.ReadAsStringAsync());
        if (rooms == null) throw new Exception("No rooms found in database.");

        var query = new QueryEntranceTestRequest
        {
            Keyword = "Room",
            IsAnnouncedScore = true,
            Shifts = new List<Shift> { Shift.Shift1_7h_8h30 }
        };
        // Act

        var queryString = new QueryStringHelper().ToQueryString(query);
        var requestUrl = $"/api/entrance-tests{queryString}";

        var response = await _client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();
        var entranceTests =
            JsonConvert.DeserializeObject<List<EntranceTestResponse>>(content, new JsonSerializerSettings());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(entranceTests);
        Assert.NotEmpty(entranceTests);
    }

    [Fact]
    public async Task GetEntranceTestStudentsWithId_WithValidId_ReturnsExpectedEntranceTest()
    {
        // Arrange
        var entranceResponse = await _client.GetAsync("/api/entrance-tests");
        var entrances =
            JsonConvert.DeserializeObject<List<RoomDetailModel>>(await entranceResponse.Content.ReadAsStringAsync());
        if (entrances == null) throw new Exception("No Entrances found in database.");

        // Act
        var requestUrl = $"/api/entrance-tests/{entrances.First().Id}";
        var response = await _client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();
        var entranceTest = JsonConvert.DeserializeObject<EntranceTestResponse>(content, new JsonSerializerSettings());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(entranceTest);
    }

    #endregion

    #region Create EntranceTest

    [Fact]
    public async Task CreateEntranceTests_WithValidValueWithUnAuthorized_ReturnNull()
    {
        // Arrange
        var roomResponse = await _client.GetAsync("/api/rooms");
        var rooms =
            JsonConvert.DeserializeObject<List<RoomDetailModel>>(await roomResponse.Content.ReadAsStringAsync());
        if (rooms == null) throw new Exception("No rooms found in database.");

        var createdEntrance = new CreateEntranceTestRequest
        {
            RoomId = rooms.First().Id,
            Shift = Shift.Shift1_7h_8h30,
            StartTime = DateTime.UtcNow.AddHours(7),
            InstructorId = "teacher002",
            RoomName = "Room 1",
            IsAnnouncedScore = false,
            RoomCapacity = 40,
            InstructorName = "Teacher 2"
        };

        // Act

        var createEntranceJson = JsonConvert.SerializeObject(createdEntrance);
        var requestContent = new StringContent(createEntranceJson, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/entrance-tests", requestContent);
        var content = await response.Content.ReadAsStringAsync();
        var entranceTest = JsonConvert.DeserializeObject<EntranceTestResponse>(content, new JsonSerializerSettings());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(entranceTest);
    }


    [Fact]
    public async Task CreateEntranceTests_WithValidValueWithAuthorized_ReturnCreatedEntranceTest()
    {
        // Arrange
        var roomResponse = await _client.GetAsync("/api/rooms");
        var rooms =
            JsonConvert.DeserializeObject<List<RoomDetailModel>>(await roomResponse.Content.ReadAsStringAsync());
        if (rooms == null) throw new Exception("No rooms found in database.");

        var createdEntrance = new CreateEntranceTestRequest
        {
            RoomId = rooms.First().Id,
            Shift = Shift.Shift1_7h_8h30,
            StartTime = DateTime.UtcNow.AddHours(7),
            InstructorId = "teacher002",
            RoomName = "Room 1",
            IsAnnouncedScore = false,
            RoomCapacity = 40,
            IsOpen = true,
            InstructorName = "Teacher 2"
        };

        // Act
        var signInRequest = new SignInRequest("quangphat7a1@gmail.com", "Quangphat12a3");
        var signInJson = JsonConvert.SerializeObject(signInRequest);
        var signInContent = new StringContent(signInJson, Encoding.UTF8, "application/json");
        var responseAuthModel = await _client.PostAsync("/api/auth/sign-in", signInContent);

        // deserialize token
        var authModel = JsonConvert.DeserializeObject<AuthModel>(await responseAuthModel.Content.ReadAsStringAsync(),
            new JsonSerializerSettings());
        var token = authModel!.IdToken;

        var createEntranceJson = JsonConvert.SerializeObject(createdEntrance);
        var requestContent = new StringContent(createEntranceJson, Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsync("/api/entrance-tests", requestContent);
        var content = await response.Content.ReadAsStringAsync();
        var entranceTest = JsonConvert.DeserializeObject<EntranceTestResponse>(content, new JsonSerializerSettings());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(entranceTest);
    }


    [Fact]
    public async Task CreateEntranceTest_WithAuthorizedUserAndInvalidRoomId_ReturnsRoomNotFound()
    {
        // Arrange
        var createdEntrance = new CreateEntranceTestRequest
        {
            RoomId = Guid.NewGuid(), // Invalid RoomId
            Shift = Shift.Shift1_7h_8h30,
            StartTime = DateTime.UtcNow.AddHours(7),
            InstructorId = "teacher002",
            RoomName = "Invalid Room",
            IsAnnouncedScore = false,
            RoomCapacity = 40,
            InstructorName = "Teacher 2"
        };

        var signInRequest = new SignInRequest("quangphat7a1@gmail.com", "Quangphat12a3");
        var signInJson = JsonConvert.SerializeObject(signInRequest);
        var signInContent = new StringContent(signInJson, Encoding.UTF8, "application/json");
        var responseAuthModel = await _client.PostAsync("/api/auth/sign-in", signInContent);

        // Deserialize token
        var authModel = JsonConvert.DeserializeObject<AuthModel>(await responseAuthModel.Content.ReadAsStringAsync(),
            new JsonSerializerSettings());
        var token = authModel!.IdToken;

        var createEntranceJson = JsonConvert.SerializeObject(createdEntrance);
        var requestContent = new StringContent(createEntranceJson, Encoding.UTF8, "application/json");

        // Act
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsync("/api/entrance-tests", requestContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Room not found.", content);
    }

    [Fact]
    public async Task CreateEntranceTest_WithAuthorizedUserAndInvalidInstructorId_ReturnsInstructorNotFound()
    {
        // Arrange
        var roomResponse = await _client.GetAsync("/api/rooms");
        var rooms =
            JsonConvert.DeserializeObject<List<RoomDetailModel>>(await roomResponse.Content.ReadAsStringAsync());
        if (rooms == null) throw new Exception("No rooms found in database.");

        var createdEntrance = new CreateEntranceTestRequest
        {
            RoomId = rooms.First().Id,
            Shift = Shift.Shift1_7h_8h30,
            StartTime = DateTime.UtcNow.AddHours(7),
            InstructorId = "teacherInvalid",
            RoomName = "Valid Room",
            IsAnnouncedScore = false,
            RoomCapacity = 40,
            InstructorName = "Teacher Invalid"
        };

        var signInRequest = new SignInRequest("quangphat7a1@gmail.com", "Quangphat12a3");
        var signInJson = JsonConvert.SerializeObject(signInRequest);
        var signInContent = new StringContent(signInJson, Encoding.UTF8, "application/json");
        var responseAuthModel = await _client.PostAsync("/api/auth/sign-in", signInContent);

        // Deserialize token
        var authModel = JsonConvert.DeserializeObject<AuthModel>(await responseAuthModel.Content.ReadAsStringAsync(),
            new JsonSerializerSettings());
        var token = authModel!.IdToken;

        var createEntranceJson = JsonConvert.SerializeObject(createdEntrance);
        var requestContent = new StringContent(createEntranceJson, Encoding.UTF8, "application/json");

        // Act
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsync("/api/entrance-tests", requestContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Account not found.", content);
    }

    #endregion

    #region Update EntranceTest

    [Fact]
    public async Task UpdateEntranceTest_WithAuthorizedUserAndValidId_UpdatesSuccessfully()
    {
        // Arrange
        var signInRequest = new SignInRequest("quangphat7a1@gmail.com", "Quangphat12a3");
        var signInJson = JsonConvert.SerializeObject(signInRequest);
        var signInContent = new StringContent(signInJson, Encoding.UTF8, "application/json");

        // Sign in to get authorization token
        var responseAuthModel = await _client.PostAsync("/api/auth/sign-in", signInContent);
        var authModel = JsonConvert.DeserializeObject<AuthModel>(await responseAuthModel.Content.ReadAsStringAsync());
        var token = authModel!.IdToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Fetch an existing EntranceTest
        var entranceTestsResponse = await _client.GetAsync("/api/entrance-tests");
        var entranceTests =
            JsonConvert.DeserializeObject<List<EntranceTestResponse>>(await entranceTestsResponse.Content
                .ReadAsStringAsync());
        if (entranceTests == null || !entranceTests.Any())
            throw new Exception("No entrance tests available for update.");

        var entranceTestToUpdate = entranceTests.First();

        var updatedEntranceTestRequest = new UpdateEntranceTestRequest
        {
            RoomId = entranceTestToUpdate.RoomId,
            Shift = Shift.Shift2_8h45_10h15,
            StartTime = entranceTestToUpdate.StartTime.AddDays(1), // Update some fields
            InstructorId = authModel.LocalId,
            RoomName = entranceTestToUpdate.RoomName,
            IsAnnouncedScore = !entranceTestToUpdate.IsAnnouncedScore
        };

        var updateEntranceTestJson = JsonConvert.SerializeObject(updatedEntranceTestRequest);
        var requestContent = new StringContent(updateEntranceTestJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/entrance-tests/{entranceTestToUpdate.Id}", requestContent);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Ensure the updated data reflects
        var updatedResponse = await _client.GetAsync($"/api/entrance-tests/{entranceTestToUpdate.Id}");
        var updatedContent = await updatedResponse.Content.ReadAsStringAsync();
        var updatedEntranceTest =
            JsonConvert.DeserializeObject<EntranceTestResponse>(updatedContent, new JsonSerializerSettings());

        Assert.NotNull(updatedEntranceTest);
        Assert.Equal(updatedEntranceTestRequest.Shift, updatedEntranceTest.Shift);
        Assert.Equal(updatedEntranceTestRequest.StartTime, updatedEntranceTest.StartTime);
        Assert.Equal(updatedEntranceTestRequest.InstructorId, updatedEntranceTest.InstructorId);
    }

    [Fact]
    public async Task UpdateEntranceTest_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var signInRequest = new SignInRequest("quangphat7a1@gmail.com", "Quangphat12a3");
        var signInJson = JsonConvert.SerializeObject(signInRequest);
        var signInContent = new StringContent(signInJson, Encoding.UTF8, "application/json");

        // Sign in to get authorization token
        var responseAuthModel = await _client.PostAsync("/api/auth/sign-in", signInContent);
        var authModel = JsonConvert.DeserializeObject<AuthModel>(await responseAuthModel.Content.ReadAsStringAsync());
        var token = authModel!.IdToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var nonExistentId = Guid.NewGuid();

        var updateEntranceTestRequest = new UpdateEntranceTestRequest
        {
            RoomId = Guid.NewGuid(),
            Shift = Shift.Shift2_8h45_10h15,
            StartTime = DateTime.UtcNow,
            InstructorId = "nonExistentTeacher",
            RoomName = "Non Existent Room",
            IsAnnouncedScore = true
        };

        var updateEntranceTestJson = JsonConvert.SerializeObject(updateEntranceTestRequest);
        var requestContent = new StringContent(updateEntranceTestJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/entrance-tests/{nonExistentId}", requestContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("This EntranceTest not found.", content);
    }

    #endregion

    #region Delete EntranceTest

    [Fact]
    public async Task DeleteEntranceTest_WithAuthorizedUserAndValidId_MarksEntranceTestAsDeleted()
    {
        // Arrange
        var signInRequest = new SignInRequest("quangphat7a1@gmail.com", "Quangphat12a3");
        var signInJson = JsonConvert.SerializeObject(signInRequest);
        var signInContent = new StringContent(signInJson, Encoding.UTF8, "application/json");

        var responseAuthModel = await _client.PostAsync("/api/auth/sign-in", signInContent);
        var authModel = JsonConvert.DeserializeObject<AuthModel>(await responseAuthModel.Content.ReadAsStringAsync());
        var token = authModel!.IdToken;

        // Set token for authorized request
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add an entrance test to delete (you might need to create it first)
        var createdEntranceTestResponse = await _client.GetAsync("/api/entrance-tests");
        var entranceTests =
            JsonConvert.DeserializeObject<List<EntranceTestResponse>>(await createdEntranceTestResponse.Content
                .ReadAsStringAsync());

        // Ensure there is at least one EntranceTest to delete
        if (entranceTests == null || !entranceTests.Any()) throw new Exception("No entrance test found to delete.");
        var entranceTestToDelete = entranceTests.First().Id;

        // Act
        var response = await _client.DeleteAsync($"/api/entrance-tests/{entranceTestToDelete}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Validate that the EntranceTest is deleted
        var getResponse = await _client.GetAsync($"/api/entrance-tests/{entranceTestToDelete}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteEntranceTest_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var signInRequest = new SignInRequest("quangphat7a1@gmail.com", "Quangphat12a3");
        var signInJson = JsonConvert.SerializeObject(signInRequest);
        var signInContent = new StringContent(signInJson, Encoding.UTF8, "application/json");

        var responseAuthModel = await _client.PostAsync("/api/auth/sign-in", signInContent);
        var authModel = JsonConvert.DeserializeObject<AuthModel>(await responseAuthModel.Content.ReadAsStringAsync());
        var token = authModel!.IdToken;


        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/entrance-tests/{nonExistentId}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("This EntranceTest not found.", content);
    }

    #endregion
}