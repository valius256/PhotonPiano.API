using System.Text.Json.Serialization;

namespace PhotonPiano.DataAccess.Models;

public class Appsettings
{
    [JsonPropertyName("ConnectionStrings")]
    public string ConnectionStrings { get; set; } = default!;

    [JsonPropertyName("FireBase")] public FireBase FireBase { get; set; } = default!;

    [JsonPropertyName("Logging")] public Logging Logging { get; set; } = default!;

    [JsonPropertyName("VnPay")] public VnPay VnPay { get; set; } = default!;

    [JsonPropertyName("AllowedHosts")] public string AllowedHosts { get; set; } = default!;

    [JsonPropertyName("Hangfire")] public string Hangfire { get; set; } = default!;

    [JsonPropertyName("SmtpAppSetting")] public SmtpAppSetting SmtpAppSetting { get; set; } = default!;

    [JsonPropertyName("FirebaseUpload")] public FirebaseUpload FirebaseUpload { get; set; } = default!;

    [JsonPropertyName("PayOs")] public PayOsOption PayOsOption { get; set; } = default!;
}

public class FirebaseUpload
{
    [JsonPropertyName("BucketName")] public string BucketName { get; set; } = default!;

    [JsonPropertyName("ProjectId")] public string ProjectId { get; set; } = default!;

    [JsonPropertyName("ServicesAccountPath")]
    public string ServicesAccountPath { get; set; } = default!;

    [JsonPropertyName("SignInPasswordKey")]
    public string SignInPasswordKey { get; set; } = default!;
}

public class SmtpAppSetting
{
    [JsonPropertyName("SmtpHost")] public string SmtpHost { get; set; } = default!;

    [JsonPropertyName("SmtpPort")] public int SmtpPort { get; set; }

    [JsonPropertyName("SmtpUserName")] public string SmtpUserName { get; set; } = default!;

    [JsonPropertyName("SmtpPassword")] public string SmtpPassword { get; set; } = default!;

    [JsonPropertyName("EnableSsl")] public bool EnableSsl { get; set; } = default!;

    [JsonPropertyName("AppVerify")] public string AppVerify { get; set; } = default!;
}

public class VnPay
{
    [JsonPropertyName("TmnCode")] public string TmnCode { get; set; } = default!;

    [JsonPropertyName("HashSecret")] public string HashSecret { get; set; } = default!;

    [JsonPropertyName("BaseUrl")] public string BaseUrl { get; set; } = default!;

    [JsonPropertyName("Command")] public string Command { get; set; } = default!;

    [JsonPropertyName("CurrCode")] public string CurrCode { get; set; } = default!;

    [JsonPropertyName("Version")] public string Version { get; set; } = default!;

    [JsonPropertyName("Locale")] public string Locale { get; set; } = default!;

    [JsonPropertyName("ReturnUrl")] public string ReturnUrl { get; set; } = default!;

    [JsonPropertyName("TimeZoneId")] public string TimeZoneId { get; set; } = default!;
}

public class Logging
{
    [JsonPropertyName("LogLevel")] public string LogLevel { get; set; } = default!;
}

public class FireBase
{
    [JsonPropertyName("ProjectId")] public string ProjectId { get; set; } = default!;
}

public class PayOsOption
{
    [JsonPropertyName("ClientID")] public string ClientID { get; set; } = default!;

    [JsonPropertyName("APIKey")] public string APIKey { get; set; } = default!;

    [JsonPropertyName("ChecksumKey")] public string ChecksumKey { get; set; } = default!;
}