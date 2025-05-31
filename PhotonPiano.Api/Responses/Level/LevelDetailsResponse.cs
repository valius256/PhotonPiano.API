using PhotonPiano.Api.Responses.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Level;

namespace PhotonPiano.Api.Responses.Level
{
    public record LevelDetailsResponse : LevelModel
    {
        public ICollection<ClassModel> Classes { get; init; } = [];

        public ICollection<AccountResponse> Accounts { get; init; } = [];
    }
}
