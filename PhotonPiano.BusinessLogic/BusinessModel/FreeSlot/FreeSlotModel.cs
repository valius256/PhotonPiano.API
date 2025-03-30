
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.FreeSlot
{
    public class FreeSlotModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public Shift Shift { get; set; }
        public string AccountId { get; set; } = string.Empty;
    }
}
