using Microsoft.EntityFrameworkCore.Storage;

namespace PhotonPiano.DataAccess.Abstractions;

public interface IUnitOfWork
{
    // repositories here 
    IEntranceTestStudentRepository EntranceTestStudentRepository { get; }

    IAccountRepository AccountRepository { get; }

    IEntranceTestRepository EntranceTestRepository { get; }

    IRoomRepository RoomRepository { get; }

    ICriteriaRepository CriteriaRepository { get; }

    ISlotRepository SlotRepository { get; }

    ITransactionRepository TransactionRepository { get; }

    IClassRepository ClassRepository { get; }

    ISystemConfigRepository SystemConfigRepository { get; }

    IStudentClassRepository StudentClassRepository { get; }

    ISlotStudentRepository SlotStudentRepository { get; }

    ITuitionRepository TuitionRepository { get; }

    IDayOffRepository DayOffRepository { get; }

    IApplicationRepository ApplicationRepository { get; }

    INotificationRepository NotificationRepository { get; }

    IAccountNotificationRepository AccountNotificationRepository { get; }
    
    IEntranceTestResultRepository EntranceTestResultRepository { get; }

    IStudentClassScoreRepository StudentClassScoreRepository { get; }
    
    ISurveyQuestionRepository SurveyQuestionRepository { get; }
    
    ILearnerSurveyRepository LearnerSurveyRepository { get; }
    
    IPianoSurveyRepository PianoSurveyRepository { get; }
    
    ILearnerAnswerRepository LearnerAnswerRepository { get; }

    ILevelRepository LevelRepository { get; }
    
    IPianoSurveyQuestionRepository PianoSurveyQuestionRepository { get; }

    Task<int> SaveChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();

    Task ExecuteInTransactionAsync(Func<Task> action);

    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

    void ClearChangeTracker();
}