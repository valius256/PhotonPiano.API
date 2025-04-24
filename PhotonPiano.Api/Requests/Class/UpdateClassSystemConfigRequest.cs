namespace PhotonPiano.Api.Requests.Class
{
    public record UpdateClassSystemConfigRequest
    {
        public int? MaximumClassSize { get; init; }
        public int? MinimumClassSize { get; init; }
        public bool? AllowSkippingLevel { get; init; }
        public int? DeadlineChangingClass { get; init; }
    }
}
