namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record UpdateEntranceTestSystemConfigModel
{
    public int? MinStudentsPerEntranceTest { get; init; }
    
    public int? MaxStudentsPerEntranceTest { get; init; }

    public bool? AllowEntranceTestRegistering { get; init; }

    public int? TestFee { get; init; }

    public int? TheoryPercentage { get; init; }
    
    public int? PracticePercentage { get; init; }
}