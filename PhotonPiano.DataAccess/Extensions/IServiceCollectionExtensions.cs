using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.DataAccess.Abtractions;
using PhotonPiano.DataAccess.Repositories;

namespace PhotonPiano.DataAccess.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
