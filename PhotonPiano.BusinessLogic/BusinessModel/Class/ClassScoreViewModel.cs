

using PhotonPiano.BusinessLogic.BusinessModel.StudentScore;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public class ClassScoreViewModel
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public bool IsScorePublished { get; set; }
    public List<StudentScoreViewModel> Students { get; set; } = new();
}