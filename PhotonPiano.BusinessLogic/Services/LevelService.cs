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
    
    public async Task<Guid> GetLevelIdFromScores(decimal theoreticalScore, decimal practicalScore)
    { 
        var level = await _unitOfWork.LevelRepository.GetLevelByScoresAsync(theoreticalScore, practicalScore);

        if (level is null)
        {
            throw new BadRequestException("Invalid score");
        }

        return level.Id;
    }
}