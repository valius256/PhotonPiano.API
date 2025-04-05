using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AccountDetailModel : AccountModel
{
    public List<LearnerSurveyWithAnswersDetailModel> LearnerSurveys { get; set; } = [];
    public List<FreeSlotModel> FreeSlots { get; set; } = [];
    public ClassModel? CurrentClass { get; init; }
    public List<StudentClassWithClassModel> StudentClasses { get; set; } = [];
    public List<EntranceTestStudentModel> EntranceTestStudents { get; init; } = [];
}