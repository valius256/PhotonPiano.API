using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record CreateClassAutoModel
    {
        public Guid Id { get; set; }

        public required Guid LevelId { get; init; }

        public required string Name { get; init; }

        public List<string> StudentIds { get; init; } = [];

        public List<CreateSlotThroughArrangementModel> Slots { get; init; } = [];
    }
}
