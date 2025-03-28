namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record CreateSurveyAnswersModel
{
    public required List<CreateLearnerAnswerModel> CreateAnswerRequests { get; init; }
}