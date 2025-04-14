using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.Scheduler
{
    public record UpdateSlotRequest
    {
        [Required]
        public required Guid Id { get; init; }
        public Shift? Shift { get; init; }
        public DateOnly? Date { get; init; }
        public Guid? RoomId { get; init; }
        public string? TeacherId { get; init; }
        public string? Reason { get; init; }

    }
}
