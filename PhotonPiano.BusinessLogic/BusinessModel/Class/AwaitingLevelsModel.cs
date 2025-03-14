using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record AwaitingLevelsModel
    {
        public Guid? Level { get; init; }
        public int Count { get; init; }
    }
}
