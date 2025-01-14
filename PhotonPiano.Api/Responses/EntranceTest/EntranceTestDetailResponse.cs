using PhotonPiano.Api.Responses.Account;
using PhotonPiano.Api.Responses.EntranceTestStudent;
using PhotonPiano.Api.Responses.Room;

namespace PhotonPiano.Api.Responses.EntranceTest;

public record EntranceTestDetailResponse : EntranceTestResponse
{
    public List<EntranceTestStudentResponse> EntranceTestStudents { get; init; } = [];

    public AccountResponse Instructor { get; init; }

    public RoomResponse Room { get; init; }
}