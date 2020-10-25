using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TempMonitor.Shared;

namespace TempMonitor.Server.Repository
{
    public interface ITemperatureRepository
    {
        Task<Temperature> GetLatestTemperature();
        Task<IList<Temperature>> GetTemperatures(DateTime? date, int? periods);
        Task SaveTemperature(Temperature temperature);
    }
}