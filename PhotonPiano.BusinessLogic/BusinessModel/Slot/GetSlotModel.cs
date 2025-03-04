using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record GetSlotModel
{
    public required DateOnly StartTime { get; init; }
    public required DateOnly EndTime { get; init; }
    public List<Shift> Shifts { get; set; } = [];
    public List<SlotStatus> SlotStatuses { get; init; } = [];
    public List<string> InstructorFirebaseIds { get; init; } = [];
    public string? StudentFirebaseId { get; init; }
    public List<Guid> ClassIds { get; init; } = [];

    // check all filter is empty
    public bool IsFilterEmpty()
    {
        return (Shifts == null || !Shifts.Any()) &&
               (SlotStatuses == null || !SlotStatuses.Any()) &&
               (InstructorFirebaseIds == null || !InstructorFirebaseIds.Any()) &&
               string.IsNullOrEmpty(StudentFirebaseId) &&
               (ClassIds == null || !ClassIds.Any());
    }

    // Method to check if a specific property is null or empty
    public bool IsPropertyNull(string propertyName)
    {
        return propertyName switch
        {
            nameof(Shifts) => Shifts == null || !Shifts.Any(),
            nameof(SlotStatuses) => SlotStatuses == null || !SlotStatuses.Any(),
            nameof(InstructorFirebaseIds) => InstructorFirebaseIds == null || !InstructorFirebaseIds.Any(),
            nameof(StudentFirebaseId) => string.IsNullOrEmpty(StudentFirebaseId),
            nameof(ClassIds) => ClassIds == null || !ClassIds.Any(),
            _ => throw new ArgumentException("Invalid property name", nameof(propertyName))
        };
    }
}