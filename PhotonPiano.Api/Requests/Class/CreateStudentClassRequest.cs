namespace PhotonPiano.Api.Requests.Class
{
    public record CreateStudentClassRequest
    {
        public required Guid ClassId { get; init; }
        public required string StudentFirebaseId { get; init; }
    }
}
