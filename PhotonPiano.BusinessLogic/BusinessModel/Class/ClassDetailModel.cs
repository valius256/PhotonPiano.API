

using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ClassDetailModel : ClassModel
    {
        public double PricePerSlots {get; set;}
        public int SlotsPerWeek {get; set; }
        public ICollection<StudentClassModel> StudentClasses { get; set; } = new List<StudentClassModel>();
        public ICollection<SlotModel> Slots { get; set; } = new List<SlotModel>();
    }
}
