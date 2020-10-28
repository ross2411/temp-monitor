using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TempMonitor.Shared;

namespace TempMonitor.Server.Controllers
{
    public class StatusController : Controller
    {
        // GET
        [HttpGet("VersionInfo")]
        public IActionResult Version()
        {
            // var version = Assembly.GetExecutingAssembly()
            //         .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0]
            //     .InformationalVersion;
            var version = ((AssemblyInformationalVersionAttribute)Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0])
                .InformationalVersion;
            return Ok(new VersionInfo
            {
                AssemblyVersion = version
            });
        }
    }
}