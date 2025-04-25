namespace PhotonPiano.Api.Requests.EntranceTest;

public record AddStudentsToEntranceTestRequest
{
    public required List<string> StudentIds { get; init; }
}