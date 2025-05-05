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

    public static string GeneratePassword(int length)
    {
        if (length <= 0)
        {
            return string.Empty;
        }
        var chars = "abcdefgijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321";
        var charsLength = chars.Length;
        var random = new Random();
        var password = string.Empty;
        for (var i = 0; i < length; i++)
        {
            var r = random.Next(charsLength - 1);
            password += chars[r];
        }
        return password;
    }
}