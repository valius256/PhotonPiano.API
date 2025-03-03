
namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record CreateStudentClassModel
    {
        public required Guid ClassId { get; init; }
        public required string StudentFirebaseId { get; init; }
    }
}
