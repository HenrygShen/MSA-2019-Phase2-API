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

        public async Task AddVideo()
        {
            await Clients.All.SendAsync("UpdateVideoList");
        }

        public async Task DeleteVideo()
        {
            await Clients.All.SendAsync("VideoDeleted");
        }
    }
}
