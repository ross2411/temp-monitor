using TempMonitor.Shared;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazorSignalRApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TempMonitor.Server.Services;

namespace TempMonitor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemperatureController : ControllerBase
    {
        private readonly IHubContext<TemperatureHub> _temperatureHubContext;
        private readonly ITemperatureService _temperatureService;
        private readonly ILogger<TemperatureController> _logger;

        public TemperatureController(
            IHubContext<TemperatureHub> hubContext,
            ITemperatureService temperatureService,
            
            ILogger<TemperatureController> logger)
        {
            this._temperatureHubContext = hubContext;
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
        [Authorize]
        public async Task<IActionResult> Save([FromBody] Temperature temperature)
        {
            await _temperatureService.SaveLatestTemperature(temperature);
            await _temperatureHubContext.Clients.All.SendAsync("LatestTempReceived", temperature);
            return this.Ok();
        }

       

        

        
    }

    

}


