

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record StudentClassDetailModel : StudentClassModel
{
    public ClassDetailModel Class { get; init; } = default!;
}