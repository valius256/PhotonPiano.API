using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.Scheduler;

public class PublicNewSlotRequest
{
    public Guid RoomId { get; init; }
    [Required]
    [DataType(DataType.Date)]
    public DateOnly Date { get; init; }
    public Shift Shift { get; init; }
    public Guid ClassId { get; init; }
}