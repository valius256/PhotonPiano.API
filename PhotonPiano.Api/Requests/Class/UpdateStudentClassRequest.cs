namespace PhotonPiano.Api.Requests.Class
{
    public record UpdateStudentClassRequest
    {
        public required Guid OldClassId { get; set; }
        public required Guid NewClassId { get; set; }
        public required string StudentFirebaseId { get; set; }
    }
}
