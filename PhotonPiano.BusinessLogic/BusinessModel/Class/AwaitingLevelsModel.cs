using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record AwaitingLevelsModel
    {
        public LevelModel? Level { get; init; }
        public int Count { get; init; }
    }
}
