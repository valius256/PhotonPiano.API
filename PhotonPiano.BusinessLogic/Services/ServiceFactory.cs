using Microsoft.Extensions.Configuration;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abtractions;
using StackExchange.Redis;

namespace PhotonPiano.BusinessLogic.Services
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly Lazy<IRedisCacheService> _redisCacheService;

        public ServiceFactory(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration configuration, IConnectionMultiplexer redis)
        {
            _redisCacheService = new Lazy<IRedisCacheService>(() => new RedisCacheService(redis));
        }

        public IRedisCacheService RedisCacheService => _redisCacheService.Value;
    }
}
