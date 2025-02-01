using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;

namespace PhotonPiano.BusinessLogic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicDependencies(this IServiceCollection services)
    {
        services.AddScoped<IServiceFactory, ServiceFactory>();

        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IEntranceTestStudentService, EntranceTestStudentService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEntranceTestService, EntranceTestService>();
        services.AddScoped<ISlotService, SlotService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<ICriteriaService, CriteriaService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ISystemConfigService, SystemConfigService>();
        services.AddScoped<ITutionService, TutionService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ISchedulerService, SchedulerService>();
        services.AddHttpClient();
        services.AddMapsterConfig();
        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        //TypeAdapterConfig<UpdateQuestionModel, Question>.NewConfig().IgnoreNullValues(true);
        //TypeAdapterConfig<UpdateQuizModel, Quiz>.NewConfig().IgnoreNullValues(true);
        //TypeAdapterConfig<Quiz, QuizWithAttemptsModel>.NewConfig()
        //    .Map(dest => dest.QuestionCnt, src => src.QuizQuestions.Count);
        return services;
    }
}