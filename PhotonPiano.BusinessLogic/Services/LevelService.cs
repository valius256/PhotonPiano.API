using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class LevelService : ILevelService
{
    private readonly IUnitOfWork _unitOfWork;

    public LevelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> GetLevelIdFromBandScore(decimal bandScore)
    {
        var level = await _unitOfWork.LevelRepository.FindFirstAsync(
            expression: l => Convert.ToDouble(bandScore) >= l.MinimumScore,
            hasTrackings: false,
            orderByExpression: l => l.MinimumScore,
            orderByDescending: true);
        
        return level?.Id ?? throw new BadRequestException("Invalid band score");
    }
}