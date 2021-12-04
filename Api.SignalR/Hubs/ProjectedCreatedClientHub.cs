using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Api.SignalR.Hubs
{
    public class ProjectedCreatedClientHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
