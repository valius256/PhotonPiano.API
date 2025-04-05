
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity
{
    public class FreeSlot : BaseEntityWithId
    {
        public DayOfWeek DayOfWeek { get; set; }
        public Shift Shift { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public virtual Account Account { get; set; } = default!;
    }
}
