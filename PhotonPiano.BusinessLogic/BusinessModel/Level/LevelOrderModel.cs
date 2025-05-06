

namespace PhotonPiano.BusinessLogic.BusinessModel.Level
{
    public class LevelOrderModel
    {
        public Guid Id { get; set; }
        public Guid? NextLevelId { get; set; } // Nullable = last level in chain
    }
}
