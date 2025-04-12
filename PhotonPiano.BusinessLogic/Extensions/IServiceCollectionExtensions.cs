using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.News;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.BusinessLogic.Services;
using PhotonPiano.DataAccess.Models.Entity;

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
        services.AddScoped<ITuitionService, TuitionService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISchedulerService, SchedulerService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IPinataService, PinataService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISurveyQuestionService, SurveyQuestionService>();
        services.AddScoped<ILearnerSurveyService, LearnerSurveyService>();
        services.AddScoped<IPianoSurveyService, PianoSurveyService>();
        services.AddScoped<ILevelService, LevelService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<ITokenService, TokenService>();
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

        TypeAdapterConfig<Class, ClassModel>
            .NewConfig()
            .Map(dest => dest.InstructorName, src => src.Instructor.UserName);

        TypeAdapterConfig<Class, ClassSimpleModel>
            .NewConfig()
            .Map(dest => dest.InstructorName, src => src.Instructor.UserName);

        TypeAdapterConfig<UpdateEntranceTestModel, EntranceTest>.NewConfig().IgnoreNullValues(true);
        TypeAdapterConfig<UpdateAccountModel, Account>.NewConfig().IgnoreNullValues(true);
        TypeAdapterConfig<UpdateSlotModel, Slot>.NewConfig().IgnoreNullValues(true);
        TypeAdapterConfig<UpdateEntranceTestResultsModel, EntranceTestStudent>.NewConfig().IgnoreNullValues(true);
        TypeAdapterConfig<UpdateSurveyQuestionModel, SurveyQuestion>.NewConfig().IgnoreNullValues(true);
        TypeAdapterConfig<UpdatePianoSurveyModel, PianoSurvey>.NewConfig().IgnoreNullValues(true);
        TypeAdapterConfig<CreateSurveyQuestionModel, SurveyQuestion>.NewConfig().Ignore(dest => dest.Id);
        TypeAdapterConfig<UpdateArticleModel, Article>.NewConfig().IgnoreNullValues(true);
        return services;
    }
}