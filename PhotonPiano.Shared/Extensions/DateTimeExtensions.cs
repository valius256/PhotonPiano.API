namespace PhotonPiano.Shared.Extensions;

public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    public static DateTime ToVietnamTime(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("DateTime must be in UTC", nameof(utcDateTime));
        }

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
    }
}