namespace PhotonPiano.Shared.Utils;

public class Constants
{
    public static List<DateTime> Holidays =
    [
        new(DateTime.UtcNow.Year, 1, 1), // New Year's Day
        new(DateTime.UtcNow.Year, 2, 28), // Tet Holiday (Lunar New Year Eve)
        new(DateTime.UtcNow.Year, 3, 1), // Tet Holiday (Lunar New Year)
        new(DateTime.UtcNow.Year, 3, 2), // Tet Holiday (3rd Day)
        new(DateTime.UtcNow.Year, 4, 30), // Reunification Day
        new(DateTime.UtcNow.Year, 5, 1), // International Workers' Day
        new(DateTime.UtcNow.Year, 9, 2), // National Day
    ];
}