﻿namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyQuestionModel
{
    public required Guid Id { get; init; }
    
    public Guid SurveyId { get; init; }

    public Guid QuestionId { get; init; }

    public int OrderIndex { get; init; }

    public bool IsRequired { get; init; }
}