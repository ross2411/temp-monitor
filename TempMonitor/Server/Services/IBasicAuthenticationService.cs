using System.Threading.Tasks;

namespace TempMonitor.Server.Services
{
    public interface IBasicAuthenticationService
    {
        Task<bool> AuthenticateUser(string username, string password);
    }
}