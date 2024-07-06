using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyWebApp.Hubs;
using MyWebApp.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly QueryLogService _queryLogService;
        private readonly IHubContext<AuditHub> _hubContext;

        public DashboardController(QueryLogService queryLogService, IHubContext<AuditHub> hubContext)
        {
            _queryLogService = queryLogService;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> LogQuery([FromBody] string queryType)
        {
            await _queryLogService.LogQueryAsync(queryType);

            var queryCounts = await _queryLogService.GetQueryCountsAsync();
            var labels = queryCounts.Keys.ToList();
            var data = queryCounts.Values.ToList();

            // Send the data to the clients
            await _hubContext.Clients.All.SendAsync("ReceiveAuditData", JsonConvert.SerializeObject(labels), JsonConvert.SerializeObject(data));

            return Ok();
        }

        public async Task<IActionResult> Index()
        {
            var queryCounts = await _queryLogService.GetQueryCountsAsync();
            ViewBag.Labels = queryCounts.Keys.ToList();
            ViewBag.Data = queryCounts.Values.ToList();

            return View();
        }
    }
}
