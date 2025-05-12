using PhotonPiano.BusinessLogic.BusinessModel.Tution;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record StudentClassWithClassAndTuitionModel : StudentClassModel
{
    public ClassModel Class { get; init; } = default!;
    
    public ICollection<TuitionModel> Tutions { get; init; } = [];
}