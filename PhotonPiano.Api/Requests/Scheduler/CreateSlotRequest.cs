using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Scheduler
{
    public record CreateSlotRequest
    {
        [Required]
        public required Shift Shift { get; init; }
        [Required]
        public required DateOnly Date { get; init; }
        [Required]
        public required Guid RoomId { get; init; }
        [Required]
        public required Guid ClassId { get; init; }
    }
}
