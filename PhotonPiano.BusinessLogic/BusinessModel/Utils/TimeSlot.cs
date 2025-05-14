using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Utils;

public record TimeSlot
{
    public DateTime Date { get; set; }

    public Shift Shift { get; set; }
}

public record TimeSlotDateOnly
{
    public DateOnly Date { get; set; }

    public Shift Shift { get; set; }
}