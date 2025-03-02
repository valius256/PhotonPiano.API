
namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public class ChangeClassModel
    {
        public required Guid OldClassId { get; set; }
        public required Guid NewClassId { get; set; }
        public required string StudentFirebaseId { get; set; }
    }
}
