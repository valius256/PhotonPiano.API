using PhotonPiano.BusinessLogic.BusinessModel.Slot;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ClassDetailModel : ClassModel
    {
        public decimal PricePerSlots { get; set; }
        public int SlotsPerWeek { get; set; }
        public ICollection<StudentClassWithClassAndTuitionModel> StudentClasses { get; set; } = new List<StudentClassWithClassAndTuitionModel>();
        public ICollection<SlotModel> Slots { get; set; } = new List<SlotModel>();
    }
}
