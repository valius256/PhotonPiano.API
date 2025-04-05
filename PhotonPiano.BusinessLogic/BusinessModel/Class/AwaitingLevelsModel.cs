using PhotonPiano.BusinessLogic.BusinessModel.Level;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record AwaitingLevelsModel
    {
        public LevelModel? Level { get; init; }
        public int Count { get; init; }
    }
}
