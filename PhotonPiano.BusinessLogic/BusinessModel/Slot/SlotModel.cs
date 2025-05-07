using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record SlotModel
{
    public Guid Id { get; init; }
    public Guid? ClassId { get; init; }
    public Guid? RoomId { get; init; }
    public Shift Shift { get; init; }
    public DateOnly Date { get; init; }
    public SlotStatus Status { get; init; }
    public string? SlotNote { get; init; }
    public string? TeacherId { get; init; }
    public AccountModel? Teacher { get; init; } 
    public RoomModel? Room { get; init; }
    public int? SlotNo { get; set; }
    public int? SlotTotal { get; set; }
}