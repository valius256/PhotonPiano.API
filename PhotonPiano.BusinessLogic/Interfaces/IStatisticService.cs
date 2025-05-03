using PhotonPiano.BusinessLogic.BusinessModel.Statistics;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IStatisticService
{
    Task<List<StatModel>> GetOverviewStatsAsync(int? month = default, int? year = default);

    Task<List<StatModel>> GetMonthlyRevenueStatsByYearAsync(int year);
    
    Task<List<PieStatModel>> GetPianoLevelStatisticsAsync(string filterBy);
}