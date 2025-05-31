using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.BusinessModel.Level;

public record LevelDetailsModel : LevelModel
{
    public ICollection<ClassModel> Classes { get; set; } = [];

    public ICollection<AccountWithTuitionModel> Accounts { get; set; } = [];
}