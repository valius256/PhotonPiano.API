using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;

namespace PhotonPiano.Api.Responses.Account;

public record AccountDetailResponse : AccountResponse
{
    public ClassModel? CurrentClass { get; set; }

    public List<StudentClassWithClassModel> StudentClasses { get; set; } = [];

    public List<LearnerSurveyWithAnswersDetailModel> LearnerSurveys { get; set; } = [];

    public List<FreeSlotModel> FreeSlots { get; set; } = [];

    public List<EntranceTestResponse> EntranceTests { get; init; } = [];

    public List<EntranceTestStudentModel> EntranceTestStudents { get; init; } = [];
}