using PhotonPiano.Api.Responses.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.Api.Responses.Level
{
    public class LevelDetailsResponse
    {
        public ICollection<ClassModel> Classes { get; set; } = [];

        public ICollection<AccountResponse> Accounts { get; set; } = [];
    }
}
