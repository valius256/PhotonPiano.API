using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Class
{
    public record UpdateClassRequest
    {
        public required Guid Id { get; init; }

        public string? Name { get; init; }

        public LevelEnum? Level { get; init; }

        public string? InstructorId { get; init; }
    }
}
