using PhotonPiano.Api.Responses.Class;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;

namespace PhotonPiano.Api.Responses.Account;

public record AccountDetailResponse : AccountResponse
{
    public ClassResponse? CurrentClass { get; init; }

    public LevelModel? Level { get; init; }

    public List<StudentClassWithClassAndTuitionModel> StudentClasses { get; init; } = [];

    public List<LearnerSurveyWithAnswersDetailModel> LearnerSurveys { get; init; } = [];

    public List<FreeSlotModel> FreeSlots { get; init; } = [];

    public List<EntranceTestStudentDetailResponse> EntranceTestStudents { get; init; } = [];
}