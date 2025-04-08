

using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account
{
    public record GrantRoleModel
    {
        public required string AccountFirebaseId { get; init; }
        public Role Role { get; init; }
    }
}
