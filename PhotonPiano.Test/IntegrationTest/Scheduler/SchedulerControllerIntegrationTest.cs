using Microsoft.AspNetCore.Mvc.Testing;
using PhotonPiano.Api.Requests.Scheduler;
using PhotonPiano.Api.Responses.Scheduler;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Models;
using PhotonPiano.Test.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static PhotonPiano.Test.Extensions.Extensions;

namespace PhotonPiano.Test.IntegrationTest.Scheduler;

//[Collection("Schedulers Integration Tests")]
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
        var response = await _client.GetAsync($"/api/scheduler/slots?start-time={DateTime.UtcNow.Date}&end-time={DateTime.UtcNow.AddDays(7)}");

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
        var request = new SchedulerRequest
        {
            StartTime = DateOnly.FromDateTime(DateTime.Now),
            EndTime = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };
        var token = await _client.GetAuthToken("learner008@gmail.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var listSlotResponseMessage = await _client.GetAsync($"/api/scheduler/slots?start-time={request.StartTime}&end-time={request.EndTime}");

        var firstSlotId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().Id;
        // Act
        var response = await _client.GetAsync($"/api/scheduler/slot/{firstSlotId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await DeserializeResponse<SlotDetailModel>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAttendance_ReturnsOkResult()
    {
        // Arrange
        var teacherLoginedToken = await _client.GetAuthToken("teacherphatlord@gmail.com", "Quangphat12a3");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", teacherLoginedToken);

        var listSlotResponseMessage = await _client.GetAsync($"/api/scheduler/slots?start-time={DateOnly.FromDateTime(DateTime.Now)}&end-time={DateOnly.FromDateTime(DateTime.Now.AddDays(7))}");
        var firstSlotId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().Id;
        var slotResponse = await _client.GetAsync($"/api/scheduler/attendance-status/{firstSlotId}");
        var slotDetails = await DeserializeResponse<List<StudentAttendanceResponse>>(slotResponse);

        var studentIds = slotDetails.Select(x => x.StudentFirebaseId).ToList();

        var slotStudentInfors = studentIds.Select(studentId => new SlotStudentInfoRequest
        {
            StudentId = studentId,
            AttendanceStatus = AttendanceStatus.Attended,
        }).ToList();

        var request = new AttendanceRequest
        {
            SlotId = firstSlotId,
            SlotStudentInfoRequests = slotStudentInfors
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/update-attendance", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>();
        Assert.NotNull(result);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task UpdateAttendance_EmptyLists_ReturnsBadRequest()
    {
        // Arrange
        var teacherLoginedToken = await _client.GetAuthToken("teacherphatlord@gmail.com", "Quangphat12a3");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", teacherLoginedToken);

        var listSlotResponseMessage = await _client.GetAsync($"/api/scheduler/slots?start-time={DateOnly.FromDateTime(DateTime.Now)}&end-time={DateOnly.FromDateTime(DateTime.Now.AddDays(7))}");
        var firstSlotId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().Id;

        var request = new AttendanceRequest
        {
            SlotId = firstSlotId,
            SlotStudentInfoRequests = new List<SlotStudentInfoRequest>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/update-attendance", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Test for invalid slot ID
    [Fact]
    public async Task UpdateAttendance_InvalidSlotId_ReturnsNotFound()
    {
        // Arrange
        var teacherLoginedToken = await _client.GetAuthToken("teacherphatlord@gmail.com", "Quangphat12a3");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", teacherLoginedToken);

        var request = new AttendanceRequest
        {
            SlotId = Guid.NewGuid(), // Invalid slot ID
            SlotStudentInfoRequests = new List<SlotStudentInfoRequest>()
            {
                new SlotStudentInfoRequest()
                {
                    StudentId = Guid.NewGuid().ToString(),
                    AttendanceStatus = AttendanceStatus.Absent
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/update-attendance", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Test for non-existent student IDs
    [Fact]
    public async Task UpdateAttendance_NonExistentStudentIds_ReturnsBadRequest()
    {
        // Arrange
        var teacherLoginedToken = await _client.GetAuthToken("teacherphatlord@gmail.com", "Quangphat12a3");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", teacherLoginedToken);

        var listSlotResponseMessage = await _client.GetAsync($"/api/scheduler/slots?start-time={DateOnly.FromDateTime(DateTime.Now)}&end-time={DateOnly.FromDateTime(DateTime.Now.AddDays(7))}");
        var firstSlotId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().Id;


        var request = new AttendanceRequest
        {
            SlotId = firstSlotId,
            SlotStudentInfoRequests = new List<SlotStudentInfoRequest>()
            {
                new SlotStudentInfoRequest()
                {
                    StudentId = Guid.NewGuid().ToString(),
                    AttendanceStatus = AttendanceStatus.Absent
                },
                new SlotStudentInfoRequest()
                {
                    StudentId = Guid.NewGuid().ToString(),
                    AttendanceStatus = AttendanceStatus.Attended
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/update-attendance", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBlankClassAndShift_ReturnsOkResult()
    {
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Arrange
        var request = new BlankSlotAndShiftRequest
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/blank-slot", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<BlankSlotModel>>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CancelSlot_ReturnsNotFound()
    {
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


        // Arrange
        var request = new CancelSlotRequest
        {
            SlotId = Guid.NewGuid(),
            CancelReason = "Test reason"
        };


        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/cancel-slot", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelSlot_ReturnsNoContent()
    {
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var listSlotResponseMessage = await _client.GetAsync($"/api/scheduler/slots?start-time={DateOnly.FromDateTime(DateTime.Now)}&end-time={DateOnly.FromDateTime(DateTime.Now.AddDays(7))}");
        var firstSlotId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().Id;


        // Arrange
        var request = new CancelSlotRequest
        {
            SlotId = firstSlotId,
            CancelReason = "Test reason" // Uncomment if needed
        };


        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/cancel-slot", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PublicNewSlot_ReturnsIllegalArgument()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var listSlotResponseMessage = await _client.GetAsync($"/api/scheduler/slots?start-time={DateOnly.FromDateTime(DateTime.Now)}&end-time={DateOnly.FromDateTime(DateTime.Now.AddDays(7))}");

        var classId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().ClassId!.Value;

        var roomId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().RoomId!.Value;


        var request = new PublicNewSlotRequest
        {
            Shift = Shift.Shift1_7h_8h30,
            ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            RoomId = roomId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/public-new-slot", request);

        // Assert
        // response.EnsureSuccessStatusCode();
        // var result = await response.Content.ReadFromJsonAsync<SlotDetailModel>();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Assert.NotNull(result);
    }

    [Fact]
    public async Task PublicNewSlot_ReturnsOk()
    {
        // Arrange
        var token = await _client.GetAuthToken("staff123@gmail.com", "Password1@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var listSlotResponseMessage = await _client.GetAsync($"/api/scheduler/slots?start-time={DateOnly.FromDateTime(DateTime.Now)}&end-time={DateOnly.FromDateTime(DateTime.Now.AddDays(7))}");

        var classId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().ClassId!.Value;

        var roomId = (await DeserializeResponse<List<SlotSimpleModel>>(listSlotResponseMessage)).First().RoomId!.Value;

        var request = new PublicNewSlotRequest
        {
            Shift = Shift.Shift3_10h45_12h,
            ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            RoomId = roomId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/scheduler/public-new-slot", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SlotDetailModel>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }
}