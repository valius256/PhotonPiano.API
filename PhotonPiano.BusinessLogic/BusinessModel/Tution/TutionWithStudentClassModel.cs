using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.BusinessModel.Tution;

public record TutionWithStudentClassModel : TutionModel
{
    public StudentClassModel StudentClass { get; init; }
}