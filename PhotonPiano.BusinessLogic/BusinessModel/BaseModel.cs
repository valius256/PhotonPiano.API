using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel;

public record BaseModel
{
    public DateTime CreatedAt { get; init; }
    
    public DateTime? UpdatedAt { get; init; }
    
    public DateTime? DeletedAt { get; init; }
    
    public RecordStatus RecordStatus { get; init; }
}
