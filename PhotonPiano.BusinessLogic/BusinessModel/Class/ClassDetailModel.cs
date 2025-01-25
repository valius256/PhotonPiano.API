

using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ClassDetailModel : ClassModel
    {
        public AccountModel Instructor { get; set; } = default!;
        public ICollection<StudentClassModel> StudentClasses { get; set; } = new List<StudentClassModel>();
        public ICollection<SlotModel> Slots { get; set; } = new List<SlotModel>();
    }
}
