using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Repositories;

namespace PhotonPiano.DataAccess.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IEntranceTestStudentRepository, EntranceTestStudentRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            
            
            
            return services;
        }
    }
}
