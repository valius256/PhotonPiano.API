

using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemConfigService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SystemConfig> GetConfig(string name)
        {
            var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == name);
            if (config is null)
            {
                throw new NotFoundException("Config not found");
            }
            return config;
        }

        public async Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel)
        {
            var config = await GetConfig(updateSystemConfigModel.ConfigName);
            config.ConfigValue = updateSystemConfigModel.ConfigValue;

            await _unitOfWork.SystemConfigRepository.UpdateAsync(config);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
