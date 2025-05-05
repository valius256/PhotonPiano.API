namespace PhotonPiano.Shared.Utils;

public class Constants
{
    public static readonly List<DateTime> Holidays =
    [
        new(DateTime.UtcNow.Year, 1, 1), // New Year's Day
        new(DateTime.UtcNow.Year, 2, 28), // Tet Holiday (Lunar New Year Eve)
        new(DateTime.UtcNow.Year, 3, 1), // Tet Holiday (Lunar New Year)
        new(DateTime.UtcNow.Year, 3, 2), // Tet Holiday (3rd Day)
        new(DateTime.UtcNow.Year, 4, 30), // Reunification Day
        new(DateTime.UtcNow.Year, 5, 1), // International Workers' Day
        new(DateTime.UtcNow.Year, 9, 2), // National Day
    ];

    public static readonly List<string> VietnameseDaysOfTheWeek =
    [
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday",
    ];

    public static readonly List<string> Shifts =
    [
        "7:00 - 8:30",
        "8:45 - 10:15",
        "10:30 - 12:00",
        "12:30 - 14:00",
        "14:15 - 15:45",
        "16:00 - 17:30",
        "18:00 - 19:30",
        "19:45 - 21:15",
    ];
    //Considering take these shift settings to config:
    //Giờ bắt đầu : 7:00
    //Độ dài ca học (phút): 90
    //Nghỉ trưa sau ca: 3
    //Độ dài nghỉ trưa (phút): 30
    //Nghỉ chiều sau ca: 6
    //Độ dài nghỉ chiều (phút): 30
    //Tổng số ca: 8
    //Khoảng cách giữa các ca (phút): 15
}