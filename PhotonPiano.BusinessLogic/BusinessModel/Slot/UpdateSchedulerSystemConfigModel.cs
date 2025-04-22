namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record UpdateSchedulerSystemConfigModel
 {
     public int? DeadlineAttendance { get; init; }
     
     public List<string>? ReasonCancelSlot { get; init; } 
     
     public decimal? MaxAbsenceRate { get; init; }
 }