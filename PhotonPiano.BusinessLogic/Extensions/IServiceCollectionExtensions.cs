using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;

namespace PhotonPiano.BusinessLogic.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLogicDependencies(this IServiceCollection services)
        {
            services.AddScoped<IServiceFactory, ServiceFactory>();

            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IEntranceTestStudentService, EntranceTestStudentService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
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
}
