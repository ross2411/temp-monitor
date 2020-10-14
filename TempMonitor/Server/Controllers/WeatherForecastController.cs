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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetFiles")]
        public IActionResult GetFiles([FromQuery]int period, [FromQuery] DateTime? date)
        {
            _logger.LogInformation("New entry in logs");
            _logger.LogInformation("Period: {0}. Date: {1}", period, date);
            var files = GetFilePaths(period, date);
            var temperatures = new List<Temperature>();
            foreach(var file in files)
            {
                _logger.LogInformation("Extracting data from {0}", file);
                if (System.IO.File.Exists(file))
                {
                    using CsvReader reader = new CsvReader(new StreamReader(file), new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
                    {
                        HasHeaderRecord = false,
                        
                    }
                    );
                    var temps = reader.GetRecords<Temperature>();
                    var lessGranular = Granularity(temps, 300);
                    temperatures.AddRange(lessGranular);
                }
                else
                {
                    _logger.LogError("Unable to find file {0}", file);
                }
            }
            return Ok(temperatures);
        }

        private IEnumerable<Temperature> Granularity(IEnumerable<Temperature> temps, int granularity)
        {
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
                    if (t.dateTime >= lastPeriod.Value.AddSeconds(granularity))
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

            //var basePath = "/var/temps";
            var basePath = "/Users/rossellerington/Projects/TempMonitor/TempMonitor/Server/Data";
            var fp = new List<string>();
            for( int i=period; i> 0; i--)
            {
                fp.Add( $"{basePath}/{date.Value.AddDays(-1 * i):dd-MM-yy}_temps.csv");
            }
            return fp;

        }

        
    }

    

}


