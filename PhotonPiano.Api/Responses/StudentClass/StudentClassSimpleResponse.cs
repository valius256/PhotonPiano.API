namespace PhotonPiano.Api.Responses.StudentClass;

public record StudentClassSimpleResponse
{
    public Guid Id { get; init; }
    public Guid ClassId { get; init; }
    public string? StudentFirebaseId { get; init; }
    public string? CertificateUrl { get; init; }
    public bool IsPassed { get; init; }
    public string? StudentFullName { get; init; }
    public string ClassName { get; init; }
    public string? StudentEmail { get; init; }
    
}