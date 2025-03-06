
namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record CreateStudentClassModel
    {
        public required Guid ClassId { get; init; }

        public required List<string> StudentFirebaseIds { get; init; }

        public bool IsAutoFill { get; init; } = false;
    }
}
