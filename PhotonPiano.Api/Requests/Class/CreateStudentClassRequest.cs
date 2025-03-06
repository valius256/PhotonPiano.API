namespace PhotonPiano.Api.Requests.Class
{
    public record CreateStudentClassRequest
    {
        public required Guid ClassId { get; init; }

        public required List<string> StudentFirebaseIds { get; init; }

        public bool IsAutoFill { get; init; } = false;
    }
}
