using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TempMonitor.Shared;

namespace TempMonitor.Server.Services
{
    public interface ITemperatureService
    {
        Task<Temperature> GetCurrentTemperature();
        Task<IList<Temperature>> GetTemperatures(DateTime? date, int? periods);
        Task SaveLatestTemperature(Temperature temperature);

    }
}