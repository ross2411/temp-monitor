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
using BlazorSignalRApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using TempMonitor.Server.Settings;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TempMonitor.Server.Services;

namespace TempMonitor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemperatureController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly ITemperatureService _temperatureService;
        private readonly ILogger<TemperatureController> _logger;
        private readonly string _basePath;

        public TemperatureController(
            IHubContext<ChatHub> hubContext,
            ITemperatureService temperatureService,
            
            ILogger<TemperatureController> logger)
        {
            this._chatHubContext = hubContext;
            this._temperatureService = temperatureService;
            this._logger = logger;
        }

        [HttpGet("GetCurrentTemp")]
        public async Task<IActionResult> GetCurrentTemperatureAsync()
        {
            var currentTemperature = await _temperatureService.GetCurrentTemperature();
            return this.Ok(currentTemperature);
        }

        [HttpGet("GetTemps")]
        public async Task<IActionResult> GetFiles([FromQuery]int? period, [FromQuery] DateTime? date)
        {
            var temperatures =  await _temperatureService.GetTemperatures(date, period);
            return this.Ok(temperatures);
        }

        [HttpPost()]
        public async Task<IActionResult> Save([FromBody] Temperature temperature)
        {
            await _temperatureService.SaveLatestTemperature(temperature);
            return this.Ok();
        }

       

        

        
    }

    

}


