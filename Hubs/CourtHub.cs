using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AquarSmartCourt.Hubs
{
    public class CourtHub : Hub
    {
        public async Task NotifyChange()
        {
            await Clients.Others.SendAsync("ReceiveUpdate");
        }
    }
}
