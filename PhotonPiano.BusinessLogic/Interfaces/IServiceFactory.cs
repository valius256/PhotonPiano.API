﻿using PhotonPiano.BackgroundJob;
using PhotonPiano.PubSub.Notification;
using PhotonPiano.PubSub.Progress;
using PhotonPiano.PubSub.StudentClassScore;

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

    ISlotService SlotService { get; }

    IPaymentService PaymentService { get; }

    IClassService ClassService { get; }

    ISystemConfigService SystemConfigService { get; }

    ISlotStudentService SlotStudentService { get; }

    ITuitionService TuitionService { get; }

    ITransactionService TransactionService { get; }

    IDefaultScheduleJob DefaultScheduleJob { get; }

    IEmailService EmailService { get; }

    ISchedulerService SchedulerService { get; }

    IApplicationService ApplicationService { get; }

    IPinataService PinataService { get; }

    INotificationService NotificationService { get; }

    INotificationServiceHub NotificationServiceHub { get; }

    IStudentClassService StudentClassService { get; }

    IDayOffService DayOffService { get; }

    IProgressServiceHub ProgressServiceHub { get; }
    

    ISurveyQuestionService ISurveyQuestionService { get; }

    ILearnerSurveyService LearnerSurveyService { get; }

    ILevelService LevelService { get; }
    
    ISurveyQuestionService SurveyQuestionService { get; }
    
    IPianoSurveyService PianoSurveyService { get; }

    IFreeSlotService FreeSlotService { get; }
    
    IArticleService ArticleService { get; }
    IStudentClassScoreService StudentClassScoreService { get; }
    ICertificateService CertificateService { get; }
    
    ITokenService TokenService { get; }
    
    IViewRenderService ViewRenderService { get; }
    
    IStudentClassScoreServiceHub StudentClassScoreServiceHub { get; }
    
    IStatisticService StatisticService { get; }
}