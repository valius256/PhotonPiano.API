using PhotonPiano.DataAccess.Models.Enum;
using System.Text.Json.Serialization;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AuthModel
{
    [JsonPropertyName("kind")] public required string Kind { get; init; }

    [JsonPropertyName("localId")] public required string LocalId { get; init; }

    [JsonPropertyName("email")] public required string Email { get; init; }

    [JsonPropertyName("displayName")] public required string DisplayName { get; init; }

    [JsonPropertyName("idToken")] public required string IdToken { get; init; }

    [JsonPropertyName("registered")] public bool Registered { get; init; }

    [JsonPropertyName("refreshToken")] public required string RefreshToken { get; init; }

    [JsonPropertyName("expiresIn")] public required string ExpiresIn { get; init; }

    [JsonPropertyName("role")] public Role Role { get; set; }
}