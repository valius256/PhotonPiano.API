using System.Runtime.Intrinsics.X86;

namespace PhotonPiano.Api.Requests.Scheduler;

public record AssignTeacherToSlotRequest
{
    public required Guid SlotId { get; init; }
    public required string TeacherFirebaseId { get; init; }
    public required string Reason { get; init; }
}