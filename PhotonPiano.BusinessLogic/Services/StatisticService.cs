using System.Globalization;
using System.Linq.Expressions;
using PhotonPiano.BusinessLogic.BusinessModel.Statistics;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class StatisticService : IStatisticService
{
    private readonly IUnitOfWork _unitOfWork;

    public StatisticService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StatModel>> GetOverviewStatsAsync(int? month = default, int? year = default)
    {
        var now = DateTime.UtcNow;

        DateTime? start = null;
        DateTime? end = null;

        if (month.HasValue || year.HasValue)
        {
            if (!month.HasValue || !year.HasValue)
                throw new BadRequestException("Both month and year must be provided.");

            if (month < 1 || month > 12)
                throw new BadRequestException("Month must be between 1 and 12.");

            var requestedStart = new DateTime(year.Value, month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            if (requestedStart > currentMonthStart)
                throw new BadRequestException("Cannot retrieve statistics for a future month.");

            start = requestedStart;
            end = requestedStart.AddMonths(1);
        }


        Expression<Func<Class, bool>> classFilter = !start.HasValue
            ? c => c.IsPublic
            : c => c.IsPublic && c.CreatedAt >= start && c.CreatedAt <= end;
        Expression<Func<Account, bool>> learnerFilter = a => a.Role == Role.Student &&
                                                             (!start.HasValue || (a.CreatedAt >= start &&
                                                                 a.CreatedAt < end));
        Expression<Func<Account, bool>> teacherFilter = a => a.Role == Role.Instructor &&
                                                             (!start.HasValue || (a.CreatedAt >= start &&
                                                                 a.CreatedAt < end));
        Expression<Func<Transaction, bool>> transactionFilter = t => t.PaymentStatus == PaymentStatus.Succeed &&
                                                                     (!start.HasValue || (t.CreatedAt >= start &&
                                                                         t.CreatedAt < end));

        var currentClassCount = await _unitOfWork.ClassRepository.CountAsync(classFilter, hasTrackings: false);
        var currentLearnerCount = await _unitOfWork.AccountRepository.CountAsync(learnerFilter, hasTrackings: false);
        var currentTeacherCount = await _unitOfWork.AccountRepository.CountAsync(teacherFilter, hasTrackings: false);

        decimal currentRevenue = 0;
        var transactions = await _unitOfWork.TransactionRepository.FindAsync(transactionFilter, hasTrackings: false);
        foreach (var t in transactions)
        {
            currentRevenue += t.TransactionType == TransactionType.Refund ? -t.Amount : t.Amount;
        }

        int compareClassCount = 0, compareLearnerCount = 0, compareTeacherCount = 0;
        decimal compareRevenue = 0;

        if (start.HasValue)
        {
            var previousStart = start.Value.AddMonths(-1);
            var previousEnd = start.Value;

            compareClassCount = await _unitOfWork.ClassRepository.CountAsync(
                c => c.IsPublic && c.CreatedAt >= previousStart && c.CreatedAt < previousEnd, hasTrackings: false);

            compareLearnerCount = await _unitOfWork.AccountRepository.CountAsync(
                a => a.Role == Role.Student &&
                     a.CreatedAt >= previousStart && a.CreatedAt < previousEnd, hasTrackings: false);

            compareTeacherCount = await _unitOfWork.AccountRepository.CountAsync(
                a => a.Role == Role.Instructor &&
                     a.CreatedAt >= previousStart && a.CreatedAt < previousEnd, hasTrackings: false);

            var prevTransactions = await _unitOfWork.TransactionRepository.FindAsync(
                t => t.PaymentStatus == PaymentStatus.Succeed &&
                     t.CreatedAt >= previousStart && t.CreatedAt < previousEnd,
                hasTrackings: false);

            foreach (var t in prevTransactions)
            {
                compareRevenue += t.TransactionType == TransactionType.Refund ? -t.Amount : t.Amount;
            }
        }

        return
        [
            new StatModel
            {
                Name = "TotalClasses",
                Value = currentClassCount,
                Unit = StatUnit.Count,
                ValueCompareToLastMonth = currentClassCount - compareClassCount,
                Month = month,
                Year = year,
            },
            new StatModel
            {
                Name = "TotalLearners",
                Value = currentLearnerCount,
                Unit = StatUnit.Count,
                ValueCompareToLastMonth = currentLearnerCount - compareLearnerCount,
                Month = month,
                Year = year,
            },
            new StatModel
            {
                Name = "TotalTeachers",
                Value = currentTeacherCount,
                Unit = StatUnit.Count,
                ValueCompareToLastMonth = currentTeacherCount - compareTeacherCount,
                Month = month,
                Year = year,
            },
            new StatModel
            {
                Name = "TotalRevenue",
                Value = currentRevenue,
                Unit = StatUnit.Count,
                ValueCompareToLastMonth = currentRevenue - compareRevenue,
                Month = month,
                Year = year,
            }
        ];
    }

    public async Task<List<StatModel>> GetMonthlyRevenueStatsByYearAsync(int year)
    {
        var now = DateTime.UtcNow.AddHours(7);

        if (year > now.Year)
        {
            throw new BadRequestException("Cannot get revenue stats for a future year.");
        }

        var allTransactions = await _unitOfWork.TransactionRepository.FindAsync(
            t => t.PaymentStatus == PaymentStatus.Succeed,
            hasTrackings: false);

        var revenueStats = new List<StatModel>();

        for (int month = 1; month <= 12; month++)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);

            if (start > now)
            {
                break;
            }

            var monthlyTransactions = allTransactions
                .Where(t => t.CreatedAt >= start && t.CreatedAt < end);

            decimal totalRevenue = monthlyTransactions.Sum(t =>
                t.TransactionType == TransactionType.Refund ? -t.Amount : t.Amount);

            revenueStats.Add(new StatModel
            {
                Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3),
                Value = totalRevenue,
                Unit = StatUnit.Count,
                Month = month,
                Year = year,
            });
        }

        return revenueStats;
    }

    public async Task<List<PieStatModel>> GetPianoLevelStatisticsAsync(string filterBy)
    {
        var levels = await _unitOfWork.LevelRepository.GetAllAsync(hasTrackings: false);

        if (filterBy.ToLower() == "classes")
        {
            var classes = await _unitOfWork.ClassRepository.FindAsync(c => c.IsPublic == true, hasTrackings: false);
            var totalClasses = classes.Count;

            var classCountsByLevel = classes.GroupBy(c => c.LevelId)
                .ToDictionary(g => g.Key, g => g.Count());

            var stats = levels.Select(level =>
            {
                classCountsByLevel.TryGetValue(level.Id, out var classCount);

                double percentage = totalClasses == 0 ? 0 : Math.Round((double)classCount * 100 / totalClasses, 2);

                return new PieStatModel
                {
                    Name = level.Name,
                    Percentage = percentage,
                    Value = classCount,
                    Color = level.ThemeColor
                };
            }).ToList();

            return stats;
        }

        var learners = await _unitOfWork.AccountRepository.FindAsync(a => a.Role == Role.Student
                                                                          && a.LevelId != null, hasTrackings: false);

        var totalLearners = learners.Count;

        var learnersCountByLevel = learners.GroupBy(l => l.LevelId ?? Guid.NewGuid())
            .ToDictionary(g => g.Key, g => g.Count());

        var learnerStats = levels.Select(level =>
        {
            learnersCountByLevel.TryGetValue(level.Id, out var classCount);

            double percentage = totalLearners == 0 ? 0 : Math.Round((double)classCount * 100 / totalLearners, 2);

            return new PieStatModel
            {
                Name = level.Name,
                Percentage = percentage,
                Value = classCount,
                Color = level.ThemeColor
            };
        }).ToList();

        return learnerStats;
    }
}