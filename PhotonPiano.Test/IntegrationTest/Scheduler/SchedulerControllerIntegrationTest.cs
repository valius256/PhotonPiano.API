using Microsoft.AspNetCore.Mvc.Testing;
using PhotonPiano.Api.Requests.Scheduler;
using PhotonPiano.Api.Responses.Scheduler;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.Shared.Models;
using PhotonPiano.Test.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static PhotonPiano.Test.Extensions.Extensions;

namespace PhotonPiano.Test.IntegrationTest.Scheduler;

public class SchedulerControllerIntegrationTest : BaseIntergrationTest
{
    private readonly HttpClient _client;



    public SchedulerControllerIntegrationTest(IntergrationTestWebAppFactory factory) : base(factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // Unauthorized
    [Fact]
    public async Task GetSchedulers_Unauthorized_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/scheduler/slots?start-time=2025-03-01&end-time=2025-03-10");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAttendanceStatus_Unauthorized_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync($"/api/scheduler/attendance-status/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Invalid input Parameters
    [Fact]
    public async Task GetSchedulers_InvalidParameters_ReturnsBadRequest()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner008@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/scheduler/slots?start-time=invalid&end-time=invalid");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Non-Existent Resources
    [Fact]
    public async Task GetSlotById_NonExistentSlot_ReturnsNotFound()
    {
        // Arrange
        var token = await _client.GetAuthToken("learner008@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/scheduler/slot/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    // Boundary Dates
    [Fact]
    public async Task GetSchedulers_BoundaryDates_ReturnsOkResult()
    {
        // Arrange
        var request = new SchedulerRequest
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now)
        };

        var token = await _client.GetAuthToken("learner008@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/scheduler/slots?start-time={request.StartTime}&end-time={request.EndTime}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await DeserializeResponse<List<SlotSimpleModel>>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }



    [Fact]
    public async Task GetSchedulers_ReturnsOkResult()
    {
        // Arrange
        var request = new SchedulerRequest
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var token = await _client.GetAuthToken("learner008@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // Act
        var response =
            await _client.GetAsync($"/api/scheduler/slots?start-time={request.StartTime}&end-time={request.EndTime}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await DeserializeResponse<List<SlotSimpleModel>>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result);

    }

    [Fact]
    public async Task GetAttendanceStatus_ReturnsOkResult()
    {
        // Arrange

        var request = new SchedulerRequest
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var token = await _client.GetAuthToken("learner008@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseSlots =
            await _client.GetAsync($"/api/scheduler/slots?start-time={request.StartTime}&end-time={request.EndTime}");

        var resultSlot = await DeserializeResponse<List<SlotSimpleModel>>(responseSlots);
        // Act
        var response = await _client.GetAsync($"/api/scheduler/attendance-status/{resultSlot.First().Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await DeserializeResponse<List<StudentAttendanceResponse>>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetSlotById_ReturnsOkResult()
    {
        // Arrange
        var slotId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/scheduler/slot/{slotId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await DeserializeResponse<SlotDetailModel>(response);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAttendance_ReturnsOkResult()
    {
        // Arrange
        var request = new AttendanceRequest
        {
            SlotId = Guid.NewGuid(),
            StudentAttentIds = new List<string> { "student1", "student2" },
            StudentAbsentIds = new List<string> { "student3" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/update-attendance", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IApiResult<bool>>();
        Assert.NotNull(result);
        Assert.True(result.Data);
    }
}