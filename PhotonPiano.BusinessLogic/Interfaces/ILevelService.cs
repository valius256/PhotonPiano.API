namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ILevelService
{
    Task<Guid> GetLevelIdFromBandScore(decimal bandScore);
}