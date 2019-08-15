using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Back_end.CentralHub
{
    public class SignalrHub : Hub
    {
        public async Task BroadcastMessage()
        {
            await Clients.All.SendAsync("Connected");
        }

        public async Task UpdateVideos()
        {
            await Clients.All.SendAsync("UpdateVideoList");
        }

        public async Task UpdateComments()
        {
            await Clients.All.SendAsync("UpdateComments");
        }
    }
}
