using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public RecordStatus RecordStatus { get; set; } = RecordStatus.IsActive;
}

public abstract class BaseEntityWithId : BaseEntity
{
    public required Guid Id { get; set; }
}