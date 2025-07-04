﻿using Microsoft.Extensions.DependencyInjection;
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
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IClassRepository, ClassRepository>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
        services.AddScoped<IDayOffRepository, DayOffRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IAccountNotificationRepository, AccountNotificationRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEntranceTestResultRepository, EntranceTestResultRepository>();
        services.AddScoped<ISurveyQuestionRepository, SurveyQuestionRepository>();
        services.AddScoped<ILearnerSurveyRepository, LearnerSurveyRepository>();
        services.AddScoped<IPianoSurveyRepository, PianoSurveyRepository>();
        services.AddScoped<ILearnerAnswerRepository, LearnerAnswerRepository>();
        services.AddScoped<IPianoSurveyQuestionRepository, PianoSurveyQuestionRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        return services;
    }
}