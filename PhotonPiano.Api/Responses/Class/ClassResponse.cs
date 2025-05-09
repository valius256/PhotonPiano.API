using PhotonPiano.Api.Responses.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Class
{
    public record ClassResponse : BaseResponse
    {
        public required Guid Id { get; init; }
        
        public string? InstructorId { get; init; }
        
        public string? InstructorName { get; init; }
        
        public ClassStatus Status { get; init; } = ClassStatus.NotStarted;
        
        public DateOnly? StartTime { get; init; }
        
        public DateOnly? EndTime { get; init; }
        
        public Guid LevelId { get; init; }
        
        public bool IsPublic { get; init; }
        
        public string? ScheduleDescription { get; init; }
        
        public required string Name { get; init; }
        
        public required string CreatedById { get; init; }
        
        public bool IsScorePublished { get; init; }
        
        public int Capacity { get; init; }
        
        public int MinimumStudents { get; init; }
        
        public int RequiredSlots { get; init; }
        
        public int TotalSlots { get; init; }
        
        public int StudentNumber { get; init; }
        
        public string? UpdateById { get; init; }
        
        public string? DeletedById { get; init; }
        
        public AccountResponse? Instructor { get; init; }

        public LevelModel? Level { get; init; }
        
    }
}
