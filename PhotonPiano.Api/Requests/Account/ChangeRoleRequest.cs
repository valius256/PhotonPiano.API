using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Account
{
    public class ChangeRoleRequest
    {
        public required string AccountFirebaseId { get; init; }
        public Role Role { get; init; }
    }
}
