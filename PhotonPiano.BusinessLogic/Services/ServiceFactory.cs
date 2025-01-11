using Microsoft.Extensions.Configuration;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using StackExchange.Redis;

namespace PhotonPiano.BusinessLogic.Services
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly Lazy<IAccountService> _accountService;

        private readonly Lazy<IAuthService> _authService;
        
        private readonly Lazy<IRedisCacheService> _redisCacheService;
        
        private readonly Lazy<IEntranceTestStudentService> _entranceTestStudentService;

        private readonly Lazy<IEntranceTestService> _entranceTestService;
        
        private readonly Lazy<IRoomService> _roomService;
        
            
        public ServiceFactory(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration configuration, IConnectionMultiplexer redis)
        {
            _accountService = new Lazy<IAccountService>(() => new AccountService(unitOfWork));
            _authService =
                new Lazy<IAuthService>(() => new AuthService(httpClientFactory, unitOfWork, this, configuration));
            _redisCacheService = new Lazy<IRedisCacheService>(() => new RedisCacheService(redis));
            _entranceTestStudentService = new Lazy<IEntranceTestStudentService>(() => new EntranceTestStudentService(unitOfWork, this));
            _entranceTestService = new Lazy<IEntranceTestService>(() => new EntranceTestService(unitOfWork, this));
            _roomService = new Lazy<IRoomService>(() => new RoomService(unitOfWork));
        }

        public IAccountService AccountService => _accountService.Value;

        public IAuthService AuthService => _authService.Value;
        
        public IRedisCacheService RedisCacheService => _redisCacheService.Value;
        
        public IEntranceTestStudentService EntranceTestStudentService => _entranceTestStudentService.Value;
        
        public IEntranceTestService EntranceTestService => _entranceTestService.Value;
        
        public IRoomService RoomService => _roomService.Value;  
    }
}
