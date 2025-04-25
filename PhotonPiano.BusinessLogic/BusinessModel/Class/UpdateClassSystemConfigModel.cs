

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record UpdateClassSystemConfigModel
    {
        public int? MaximumClassSize { get; init; }
        public int? MinimumClassSize { get; init; }
        public bool? AllowSkippingLevel { get; init; }
        public int? DeadlineChangingClass { get; init; }
    }
}
