
namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ShiftClassScheduleModel
    {
        public Guid ClassId { get; init; }

        public int Weeks { get; init; }
    }
}
