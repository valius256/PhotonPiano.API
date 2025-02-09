
namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record CreateStudentClassModel
    {
        public Guid ClassId { get; init; }
        public string? StudentFirebaseId { get; init; }
    }
}
