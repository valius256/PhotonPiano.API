namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IServiceFactory
    {
        IRedisCacheService RedisCacheService { get; }
    }
}
