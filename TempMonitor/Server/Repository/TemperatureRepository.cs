﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TempMonitor.Server.Settings;
using TempMonitor.Shared;

namespace TempMonitor.Server.Repository
{
    public class TemperatureRepository: ITemperatureRepository
    {
        private readonly ILogger<TemperatureRepository> _logger;
        private readonly string _basePath;
        public TemperatureRepository(
            IOptions<TemperatureSettings> temperatureSettings,
            ILogger<TemperatureRepository> logger)
        {
            _logger = logger;
            _basePath = temperatureSettings.Value.BasePath;
        }

        public async Task<Temperature> GetLatestTemperature()
        {
            var now = DateTime.Now;
            var filePath = GetFilePaths(1, now).Single();
            var temps = ExtractTemperatures(filePath, out var reader);
            using (reader)
            {
                return (await temps.ToListAsync()).Last();
            }
        }

        public async Task<IList<Temperature>> GetTemperatures(DateTime? date, int? periods)
        {
            _logger.LogInformation("New entry in logs");
            _logger.LogInformation("Period: {0}. Date: {1}", periods, date);
            CsvReader reader = null;
            if (periods == null)
            {
                _logger.LogInformation("No period was specified so defaulting to today");
                periods = 1;
            }
            var files = GetFilePaths(periods.Value, date);
            var temperatures = new List<Temperature>();
            foreach(var file in files)
            {
                _logger.LogInformation("Extracting data from {0}", file);

                var temps = ExtractTemperatures(file, out reader);
                var granulatedTemps = await Granularity(temps, periods ?? 1);
                temperatures.AddRange(granulatedTemps);
               
            }
            using (reader)
            {
                temperatures = temperatures.Where(t => t.OutsideTemp.HasValue).ToList();
                return temperatures.ToList();
            }
           
        }

        public Task SaveTemperature(Temperature temperature)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetFilePaths(int period, DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Now;

            var fp = new List<string>();
            for (int i = period - 1; i >= 0; i--)
            {
                fp.Add($"{_basePath}/{date.Value.AddDays(-1 * i):dd-MM-yy}_temps.csv");
            }
            return fp;

        }

        private IAsyncEnumerable<Temperature> ExtractTemperatures(string file, out CsvReader reader)
        {
            if (!File.Exists(file))
            {
                _logger.LogError("Unable to find file {0}", file);
            }

            reader = new CsvReader(new StreamReader(file), new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
            {
                HasHeaderRecord = false,
                MissingFieldFound = null,
            }
                   );
            var temps =  reader.GetRecordsAsync<Temperature>();
            return temps;

        }
        
        private async Task<IEnumerable<Temperature>> Granularity(IAsyncEnumerable<Temperature> temps, int period)
        {
            //For 1 day I think 30 min granularity work which means add 30 mins to each period but multiply this by period to get the granularity
            var granularity = 30 * period;

            List<Temperature> toReturn = new List<Temperature>();
            DateTime? lastPeriod = null;
            await foreach (var t in temps)
            {
                if (lastPeriod == null)
                {
                    lastPeriod = t.dateTime;
                    toReturn.Add(t);
                }
                else
                {
                    if (t.dateTime >= lastPeriod.Value.AddMinutes(granularity))
                    {
                        toReturn.Add(t);
                        lastPeriod = t.dateTime;
                    }
                }
            }
            return toReturn;
        }

       
    }
}