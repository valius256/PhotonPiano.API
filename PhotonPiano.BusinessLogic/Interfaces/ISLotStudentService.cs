using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISlotStudentService
{
    Task<bool> UpdateAttentStudent(UpdateAttentdanceModel model, string teacherId);
}