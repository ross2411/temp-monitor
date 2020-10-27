using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TempMonitor.Shared;

namespace BlazorSignalRApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendLatestTemp(Temperature temperature)
        {
            await Clients.All.SendAsync("SendLatestTemp", temperature);
        }
    }

    public class TemperatureHub : Hub
    {
        
    }
}