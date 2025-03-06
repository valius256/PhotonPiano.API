
namespace PhotonPiano.BusinessLogic.BusinessModel.DayOff
{
    public class DayOffModel
    {
        public required Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
