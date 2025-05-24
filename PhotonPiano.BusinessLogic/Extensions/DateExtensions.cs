using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.Extensions;

public static class DateExtensions
{
    private static readonly Dictionary<Shift, (TimeSpan Start, TimeSpan End)> ShiftTimes = new()
    {
        { Shift.Shift1_7h_8h30, (new TimeSpan(7, 0, 0), new TimeSpan(8, 30, 0)) },
        { Shift.Shift2_8h45_10h15, (new TimeSpan(8, 45, 0), new TimeSpan(10, 15, 0)) },
        { Shift.Shift3_10h45_12h, (new TimeSpan(10, 45, 0), new TimeSpan(12, 0, 0)) },
        { Shift.Shift4_12h30_14h00, (new TimeSpan(12, 30, 0), new TimeSpan(14, 0, 0)) },
        { Shift.Shift5_14h15_15h45, (new TimeSpan(14, 15, 0), new TimeSpan(15, 45, 0)) },
        { Shift.Shift6_16h00_17h30, (new TimeSpan(16, 0, 0), new TimeSpan(17, 30, 0)) },
        { Shift.Shift7_18h_19h30, (new TimeSpan(18, 0, 0), new TimeSpan(19, 30, 0)) },
        { Shift.Shift8_19h45_21h15, (new TimeSpan(19, 45, 0), new TimeSpan(21, 15, 0)) }
    };

    public static string FormatDays(ICollection<Slot> slots)
    {
        var days = slots
            .Select(s => s.Date.DayOfWeek)
            .Distinct()
            .OrderBy(d => d)
            .Select(d => d.ToString().Substring(0, 3)) // Get English name of the day
            .ToList();

        return string.Join("/", days); 
    }

    public static List<string> FormatTime(ICollection<Slot> slots)
    {
        var shiftTimes = slots
            .Where(s => ShiftTimes.ContainsKey(s.Shift))
            .Select(s => ShiftTimes[s.Shift])
            .Distinct()
            .OrderBy(t => t.Start)
            .ToList();

        var result = new List<string>();
        foreach (var shift in shiftTimes) result.Add($"{shift.Start:hh\\:mm} - {shift.End:hh\\:mm}");
        return result;
    }
}