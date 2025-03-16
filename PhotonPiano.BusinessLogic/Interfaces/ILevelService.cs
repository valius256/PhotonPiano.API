namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ILevelService
{
    Task<Guid> GetLevelIdFromScores(decimal theoreticalScore, decimal practicalScore);
}