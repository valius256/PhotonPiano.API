

using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey
{
    public record LearnerSurveyWithAnswersDetailModel : LearnerSurveyModel
    {
        public List<LearnerAnswerWithQuestionModel> LearnerAnswers { get; init; } = [];
        public PianoSurveyModel PianoSurvey { get; set; } = default!;
    }
}
