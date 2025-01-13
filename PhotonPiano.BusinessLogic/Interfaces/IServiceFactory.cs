namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IServiceFactory
{
    IRedisCacheService RedisCacheService { get; }

    IAuthService AuthService { get; }

    IAccountService AccountService { get; }

    IEntranceTestStudentService EntranceTestStudentService { get; }

    IEntranceTestService EntranceTestService { get; }

    IRoomService RoomService { get; }

    ICriteriaService CriteriaService { get; }
}