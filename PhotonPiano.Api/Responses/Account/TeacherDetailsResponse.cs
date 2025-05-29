using PhotonPiano.Api.Responses.Class;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.Api.Responses.Account;

public record TeacherDetailsResponse : AccountResponse
{
    public List<EntranceTestResponse> InstructorEntranceTests { get; init; } = [];
    public List<ClassResponse> InstructorClasses { get; init; } = [];
}