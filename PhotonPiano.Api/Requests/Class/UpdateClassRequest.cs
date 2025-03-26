namespace PhotonPiano.Api.Requests.Class
{
    public record UpdateClassRequest
    {
        public required Guid Id { get; init; }

        public string? Name { get; init; }

        public Guid? LevelId { get; init; }

        public string? InstructorId { get; init; }
    }
}
