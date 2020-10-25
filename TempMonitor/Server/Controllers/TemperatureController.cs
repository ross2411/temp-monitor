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
            //await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", "TempController", "Get Current Temp was just called");
            //var now = DateTime.Now;
            //var filePath = GetFilePaths(1, now).First();
            //var temps = ExtractTemperatures(filePath);
            //if (!temps.Any())
            //    return this.BadRequest(new { ErrorMessage = "No Temperature files exist for the date selected"});
            //return this.Ok(temps.Last());
            var currentTemperature = await _temperatureService.GetCurrentTemperature();
            return this.Ok(currentTemperature);
        }

        [HttpGet("GetTemps")]
        public async Task<IActionResult> GetFiles([FromQuery]int? period, [FromQuery] DateTime? date)
        {
            var temperatures =  await _temperatureService.GetTemperatures(date, period);
            return this.Ok(temperatures);
        }

       

        

        
    }

    

}


