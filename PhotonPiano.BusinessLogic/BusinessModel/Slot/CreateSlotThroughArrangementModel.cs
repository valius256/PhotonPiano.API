
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot
{
    public class CreateSlotThroughArrangementModel
    {
        public Shift Shift { get; set; }
        public DateOnly Date { get; set; }
    }
}
