using PhotonPiano.Api.Responses.Account;
using PhotonPiano.Api.Responses.Room;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

namespace PhotonPiano.Api.Responses.EntranceTest;

public record EntranceTestDetailResponse : EntranceTestResponse
{
    public List<EntranceTestStudentWithResultsModel> EntranceTestStudents { get; init; } = [];

    public AccountResponse Instructor { get; init; } = default!;

    public RoomResponse Room { get; init; } = default!;
}