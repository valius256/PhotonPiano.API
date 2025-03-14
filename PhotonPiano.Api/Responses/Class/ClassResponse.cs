using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Class
{
    public record ClassResponse
    {
        public string? InstructorId { get; init; }
        public ClassStatus Status { get; init; }
        public LevelEnum? Level { get; init; }
        public bool IsPublic { get; init; }
        public required string Name { get; init; }
        public bool IsScorePublished { get; init; }
    }
}
