using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Class
{
    public record ShiftClassScheduleRequest
    {
        public required Guid ClassId { get; init; }

        [Range(1,52)]
        public required int Weeks { get; init; }
    }
}
