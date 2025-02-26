using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.BusinessModel.Tution;

public record TuitionWithStudentClassModel : TuitionModel
{
    public StudentClassModel StudentClass { get; init; }
}