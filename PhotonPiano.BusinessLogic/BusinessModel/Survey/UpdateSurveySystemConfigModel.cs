namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record UpdateSurveySystemConfigModel
{
    public string? InstrumentName { get; init; }

    public int? InstrumentFrequencyInResponse { get; init; }

    public string? EntranceSurveyId { get; init; }

    public int? MaxQuestionsPerSurvey { get; init; }
    
    public int? MinQuestionsPerSurvey { get; init; }

    public void Deconstruct(out string? instrumentName, out int? instrumentFrequencyInResponse, out string? entranceSurveyId, out int? maxQuestionsPerSurvey, out int? minQuestionsPerSurvey)
    {
        instrumentName = InstrumentName;
        instrumentFrequencyInResponse = InstrumentFrequencyInResponse;
        entranceSurveyId = EntranceSurveyId;
        maxQuestionsPerSurvey = MaxQuestionsPerSurvey;
        minQuestionsPerSurvey = MinQuestionsPerSurvey;
    }
}