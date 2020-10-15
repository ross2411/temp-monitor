using TempMonitor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace TempMonitor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly string _basePath;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            this._logger = logger;
            this._basePath = "/var/temps";
            //this._basePath = "/Users/rossellerington/Projects/TempMonitor/TempMonitor/Server/Data";
        }

        [HttpGet("GetCurrentTemp")]
        public IActionResult GetCurrentTemperature()
        {
            var now = DateTime.Now;
            var filePath = GetFilePaths(1, now).First();
            var temps = ExtractTemperatures(filePath, 1);
            return this.Ok(temps.Last());
        }

        [HttpGet("GetFiles")]
        public IActionResult GetFiles([FromQuery]int? period, [FromQuery] DateTime? date)
        {
            _logger.LogInformation("New entry in logs");
            _logger.LogInformation("Period: {0}. Date: {1}", period, date);
            if (period == null)
            {
                _logger.LogInformation("No period was specified so defaulting to today");
                period = 1;
            }
            var files = GetFilePaths(period.Value, date);
            var temperatures = new List<Temperature>();
            foreach(var file in files)
            {
                _logger.LogInformation("Extracting data from {0}", file);
                if (System.IO.File.Exists(file))
                {
                    var temps = ExtractTemperatures(file, period);
                    temperatures.AddRange(temps);
                }
                else
                {
                    _logger.LogError("Unable to find file {0}", file);
                }
            }
            temperatures = temperatures.Where(t => t.OutsideTemp.HasValue).ToList();
            return Ok(temperatures);
        }

        private IEnumerable<Temperature> ExtractTemperatures(string file, int? period)
        {
            using CsvReader reader = new CsvReader(new StreamReader(file), new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
            {
                HasHeaderRecord = false,
                MissingFieldFound = null,
            }
                   );
            var temps = reader.GetRecords<Temperature>();
            return Granularity(temps, period.Value);

        }

        private IEnumerable<Temperature> Granularity(IEnumerable<Temperature> temps, int period)
        {
            //For 1 day I think 30 min granularity work which means add 30 mins to each period but multiply this by period to get the granularity
            var granularity = 30 * period;

            List<Temperature> toReturn = new List<Temperature>();
            DateTime? lastPeriod = null;
            foreach (var t in temps)
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

        private IEnumerable<string> GetFilePaths(int period, DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Now;

            var fp = new List<string>();
            for( int i=period-1; i>= 0; i--)
            {
                fp.Add( $"{_basePath}/{date.Value.AddDays(-1 * i):dd-MM-yy}_temps.csv");
            }
            return fp;

        }

        
    }

    

}


