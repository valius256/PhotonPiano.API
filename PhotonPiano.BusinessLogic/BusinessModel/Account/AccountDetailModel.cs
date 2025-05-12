using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AccountDetailModel : AccountModel
{
    public List<LearnerSurveyWithAnswersDetailModel> LearnerSurveys { get; set; } = [];
    
    public List<FreeSlotModel> FreeSlots { get; init; } = [];
    public ClassWithSlotsModel? CurrentClass { get; init; }
    public List<StudentClassWithClassAndTuitionModel> StudentClasses { get; init; } = [];
    public List<EntranceTestStudentDetail> EntranceTestStudents { get; init; } = [];
}