using System.Security.Cryptography;
using System.Text;

namespace PhotonPiano.Shared.Utils;

public static class AuthUtils
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        // Convert the password string to bytes
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        // Compute the hash
        byte[] hashBytes = sha256.ComputeHash(passwordBytes);

        // Convert the hash to a hexadecimal string
        string hashedPassword = string.Concat(hashBytes.Select(b => $"{b:x2}"));

        return hashedPassword;
    }

    public static string GenerateSecureToken(int size = 64)
    {
        var tokenBytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }

        return Convert.ToBase64String(tokenBytes)
                      .Replace("+", "-")
                      .Replace("/", "_")
                      .Replace("=", ""); // URL-safe
    }
}