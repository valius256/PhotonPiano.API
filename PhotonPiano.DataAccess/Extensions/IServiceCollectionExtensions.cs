using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Repositories;

namespace PhotonPiano.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IEntranceTestStudentRepository, EntranceTestStudentRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IEntranceTestRepository, EntranceTestRepository>();
        services.AddScoped<ISlotRepository, SlotRepository>();

        return services;
    }
}