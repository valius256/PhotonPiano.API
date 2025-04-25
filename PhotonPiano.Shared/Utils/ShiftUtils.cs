using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Shared.Utils;

public static class ShiftUtils
{
    private static TimeOnly GetShiftStartTime(Shift shift)
    {
        return shift switch
        {
            Shift.Shift1_7h_8h30 => new TimeOnly(7, 0),
            Shift.Shift2_8h45_10h15 => new TimeOnly(8, 45),
            Shift.Shift3_10h45_12h => new TimeOnly(10, 45),
            Shift.Shift4_12h30_14h00 => new TimeOnly(12, 30),
            Shift.Shift5_14h15_15h45 => new TimeOnly(14, 15),
            Shift.Shift6_16h00_17h30 => new TimeOnly(16, 0),
            Shift.Shift7_18h_19h30 => new TimeOnly(18, 0),
            Shift.Shift8_19h45_21h15 => new TimeOnly(19, 45),
            _ => throw new ArgumentOutOfRangeException(nameof(shift), shift, null)
        };
    }

    private static TimeOnly GetShiftEndTime(Shift shift)
    {
        return shift switch
        {
            Shift.Shift1_7h_8h30 => new TimeOnly(8, 30),
            Shift.Shift2_8h45_10h15 => new TimeOnly(10, 15),
            Shift.Shift3_10h45_12h => new TimeOnly(12, 0),
            Shift.Shift4_12h30_14h00 => new TimeOnly(14, 0),
            Shift.Shift5_14h15_15h45 => new TimeOnly(15, 45),
            Shift.Shift6_16h00_17h30 => new TimeOnly(17, 30),
            Shift.Shift7_18h_19h30 => new TimeOnly(19, 30),
            Shift.Shift8_19h45_21h15 => new TimeOnly(21, 15),
            _ => throw new ArgumentOutOfRangeException(nameof(shift), shift, null)
        };
    }

    private static bool HasShiftEnded(DateOnly dateToCompare, Shift shift)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

        if (today > dateToCompare)
        {
            return true;
        }

        if (today == dateToCompare)
        {
            var shiftEndTime = GetShiftEndTime(shift);
            var shiftEndDateTime = dateToCompare.ToDateTime(shiftEndTime);

            return DateTime.Now > shiftEndDateTime;
        }   

        return false;
    }
    
    public static EntranceTestStatus GetEntranceTestStatus(DateOnly testDate, Shift shift)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var now = DateTime.UtcNow.AddHours(7);

        if (today < testDate)
        {
            return EntranceTestStatus.NotStarted;
        }

        if (today > testDate)
        {
            return EntranceTestStatus.Ended;
        }
        
        var startTime = GetShiftStartTime(shift);
        var endTime = GetShiftEndTime(shift);

        var shiftStartDateTime = testDate.ToDateTime(startTime);
        var shiftEndDateTime = testDate.ToDateTime(endTime);

        if (now < shiftStartDateTime)
        {
            return EntranceTestStatus.NotStarted;
        }

        if (now >= shiftStartDateTime && now <= shiftEndDateTime)
        {
            return EntranceTestStatus.OnGoing;
        }

        return EntranceTestStatus.Ended;
    }
}