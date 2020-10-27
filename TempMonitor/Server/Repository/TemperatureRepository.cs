using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TempMonitor.Server.CsvMapper;
using TempMonitor.Server.Settings;
using TempMonitor.Shared;
using System.IO.Abstractions;

namespace TempMonitor.Server.Repository
{
    public class TemperatureRepository: ITemperatureRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<TemperatureRepository> _logger;
        private readonly string _basePath;
        public TemperatureRepository(
            IOptions<TemperatureSettings> temperatureSettings,
            IFileSystem fileSystem,
            ILogger<TemperatureRepository> logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _basePath = temperatureSettings.Value.BasePath;
        }

        public async Task<Temperature> GetLatestTemperature()
        {
            var now = DateTime.Now;
            var files = _fileSystem.DirectoryInfo.FromDirectoryName(_basePath).GetFiles().OrderByDescending(m => m.LastWriteTime);

            var filePath = files.First().FullName;
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

        public async Task SaveTemperature(Temperature temperature)
        {
            var filePath = GetFilePath(temperature.dateTime);
            if (!_fileSystem.File.Exists(filePath))
                _fileSystem.File.Create(filePath);
            await using Stream fileStream = _fileSystem.FileStream.Create(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            await using StreamWriter streamWriter = new StreamWriter(fileStream);
            await using var csv = new CsvWriter(streamWriter, GetConfig());
            // Append records to csv
            csv.WriteRecord(temperature);
            await csv.NextRecordAsync();
            _logger.LogInformation("Added new temperature. Internal Temp: {0}, External Temp: {1}", temperature.InsideTemp, temperature.OutsideTemp);

        }

        private IEnumerable<string> GetFilePaths(int period, DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Now;

            var fp = new List<string>();
            for (int i = period - 1; i >= 0; i--)
            {
                fp.Add(GetFilePath(date.Value.AddDays(-1 * i)));
            }
            return fp;
        }

        private string GetFilePath(DateTime date)
        {
            return $"{_basePath}/{date:dd-MM-yy}_temps.csv";
        }

        private IAsyncEnumerable<Temperature> ExtractTemperatures(string file, out CsvReader reader)
        {
            if (!_fileSystem.File.Exists(file))
            {
                _logger.LogError("Unable to find file {0}", file);
            }

            reader = new CsvReader(new StreamReader(file), GetConfig());
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

        private CsvConfiguration GetConfig()
        {
            var config = new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
            {
                HasHeaderRecord = false,
                MissingFieldFound = null,
            };
            config.RegisterClassMap<TemperatureCsvMapper>();
            return config;
        }

       
    }
}
