namespace PhotonPiano.Api.Requests.Class
{
    public record CreateClassRequest
    {
        public required Guid LevelId { get; init; }
    }
}
