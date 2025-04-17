using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses;

public record BaseResponse
{
    public DateTime CreatedAt { get; init; }
    
    public DateTime? UpdatedAt { get; init; }
    
    public DateTime? DeletedAt { get; init; }
    
    public RecordStatus RecordStatus { get; init; }
}