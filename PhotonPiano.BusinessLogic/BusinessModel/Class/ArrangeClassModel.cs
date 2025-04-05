

using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ArrangeClassModel
    {
        //public List<Shift> AllowedShifts { get; init; } = [];

        public DateOnly StartWeek { get; init; }
    }
}
