using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TempMonitor.Server.Settings;

namespace TempMonitor.Server.Services
{
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        private readonly BasicAuthenticationSettings _settings;

        public BasicAuthenticationService(IOptions<BasicAuthenticationSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task<bool> AuthenticateUser(string username, string password)
        {
            return await Task.FromResult(username != _settings.UserName || password != _settings.Password);
        }
    }
}