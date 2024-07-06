using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MyWebApp.Hubs
{
    public class AuditHub : Hub
    {
        public async Task SendAuditData(string labels, string data)
        {
            await Clients.All.SendAsync("ReceiveAuditData", labels, data);
        }
    }
}
