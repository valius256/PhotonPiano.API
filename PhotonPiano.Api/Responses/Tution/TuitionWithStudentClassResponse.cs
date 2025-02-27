using PhotonPiano.Api.Responses.StudentClass;

namespace PhotonPiano.Api.Responses.Tution;

public record TuitionWithStudentClassResponse : TutionResponse
{
    public StudentClassSimpleResponse StudentClass { get; set; }
}