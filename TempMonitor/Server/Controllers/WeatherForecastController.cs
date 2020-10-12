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

        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            this.logger = logger;
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
        public IActionResult GetFiles([FromQuery]int period)
        {
            logger.LogInformation("Period: {0}", period);
            var files = GetFilePaths(period);
            var temperatures = new List<Temperature>();
            foreach(var file in files)
            {
                logger.LogInformation("Extracting data from {0}", file);
                if (System.IO.File.Exists(file))
                {
                    using CsvReader reader = new CsvReader(new StreamReader(file), new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        HasHeaderRecord = false
                    }
                    );
                    temperatures.AddRange(reader.GetRecords<Temperature>());
                }
            }
            
            return this.Ok(temperatures);
        }

        private IEnumerable<string> GetFilePaths(int period)
        {
            var basePath = "/var/temps";
            var today = DateTime.Now;
            var fp = new List<string>();
            for( int i=period; i> 0; i--)
            {
                fp.Add( $"{basePath}/{today.AddDays(-1 * i):dd-MM-yy}_temps.csv");
            }
            return fp;

        }

        
    }

    

}


