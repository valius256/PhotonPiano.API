namespace PhotonPiano.Api.Responses.StudentClass;

public class StudentClassSimpleResponse
{
    public Guid Id { get; set; }
    public Guid ClassId { get; init; }
    public string? StudentFirebaseId { get; init; }
    public string? CertificateUrl { get; init; }
    public bool IsPassed { get; init; }
    public string? StudentFullName { get; init; }
    public string ClassName { get; init; }
}