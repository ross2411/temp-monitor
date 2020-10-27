using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TempMonitor.Server.Repository;
using TempMonitor.Shared;

namespace TempMonitor.Server.Services
{
    public class TemperatureService : ITemperatureService
    {
        private readonly ITemperatureRepository _temperatureRepository;
        private readonly ILogger<TemperatureService> _logger;

        public TemperatureService(
            ITemperatureRepository temperatureRepository,
            ILogger<TemperatureService> logger)
        {
            _temperatureRepository = temperatureRepository;
            _logger = logger;
        }

        public Task<Temperature> GetCurrentTemperature()
        {
            return this._temperatureRepository.GetLatestTemperature();
        }

        public Task<IList<Temperature>> GetTemperatures(DateTime? date, int? periods)
        {
            return _temperatureRepository.GetTemperatures(date, periods);
        }

        public async Task SaveLatestTemperature(Temperature temperature)
        {
            await _temperatureRepository.SaveTemperature(temperature);
        }
    }
}
