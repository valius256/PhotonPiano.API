using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;

namespace PhotonPiano.Api.Responses.Account;

public record AccountDetailResponse : AccountResponse
{
    public ClassModel? CurrentClass { get; init; }

    public LevelModel? Level { get; init; }

    public List<StudentClassWithClassModel> StudentClasses { get; init; } = [];

    public List<LearnerSurveyWithAnswersDetailModel> LearnerSurveys { get; init; } = [];

    public List<FreeSlotModel> FreeSlots { get; init; } = [];

    public List<EntranceTestResponse> EntranceTests { get; init; } = [];

    public List<EntranceTestStudentModel> EntranceTestStudents { get; init; } = [];
}