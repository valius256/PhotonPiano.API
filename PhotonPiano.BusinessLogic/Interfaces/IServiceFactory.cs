namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IServiceFactory
    {
        IRedisCacheService RedisCacheService { get; }
        
        IAuthService AuthService { get; }
        
        IEntranceTestStudentService EntranceTestStudentService { get; }
        
        IAccountService AccountService { get; }
    }
}
