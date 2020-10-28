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
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is { })
                return this.Ok(new VersionInfo
                {
                    AssemblyVersion = version.ToString()
                });
            else
            {
                return this.BadRequest(new
                {
                    ErrorMessage = "Unable to query Assembly version"
                });
            }
        }
    }
}