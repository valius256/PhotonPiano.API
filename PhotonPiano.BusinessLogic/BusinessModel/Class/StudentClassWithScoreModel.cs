

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record StudentClassWithScoreModel : StudentClassModel
    {
        public List<StudentClassScoreModel> StudentClassScores { get; set; } = [];
    }
}
