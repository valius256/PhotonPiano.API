
namespace PhotonPiano.BusinessLogic.BusinessModel.Auth
{
    public record ChangePasswordModel
    {
        public required string Password { get; init; }
        public required string Email { get; init; }
        public required string ResetPasswordToken { get; init; }
    }
}
