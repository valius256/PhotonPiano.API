using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey
{
    public record LearnerAnswerWithQuestionModel : LearnerAnswerModel
    {
        public SurveyQuestionModel SurveyQuestion { get; init; } = default!;
    }
}
