using PhotonPiano.Api.Responses.StudentClass;

namespace PhotonPiano.Api.Responses.Tution;

public record TutionWithStudentClassResponse : TutionResponse
{
    public StudentClassSimpleResponse StudentClass { get; set; }
}