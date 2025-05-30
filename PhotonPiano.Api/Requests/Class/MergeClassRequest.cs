namespace PhotonPiano.Api.Requests.Class
{
    public record MergeClassRequest
    {
        public Guid SourceClassId { get; init; }
        public Guid TargetClassId { get; init; }
    }
}
