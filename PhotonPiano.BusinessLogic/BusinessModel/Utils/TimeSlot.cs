using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Utils;

public record TimeSlot
{
    public DateTime Date { get; set; }
    
    public Shift Shift { get; set; }
}