

using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account
{
    public record AccountWithTuitionModel : AccountModel
    {
        public List<StudentClassWithClassAndTuitionModel> StudentClasses { get; init; } = [];
    }
}
