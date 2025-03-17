using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Survey;

public record CreatePianoSurveyRequest
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }

    public string? Description { get; init; }
}