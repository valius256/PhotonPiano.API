

using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record UpdateClassModel
    {
        public required Guid Id { get; init; }

        public string? Name { get; init; }

        public string? ScheduleDescription { get; init; }

        public Guid? LevelId { get; init; }

        public string? InstructorId { get; init; }

    }
}
