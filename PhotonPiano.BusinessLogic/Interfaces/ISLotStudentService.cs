
using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISLotStudentService
{
    Task UpdateAttentStudent(UpdateAttentdanceModel model, string teacherId);
}