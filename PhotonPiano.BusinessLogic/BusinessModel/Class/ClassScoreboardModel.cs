

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ClassScoreboardModel : ClassModel
    {
        public List<StudentClassWithScoreModel> StudentClasses { get; init; } = [];
    }
}
