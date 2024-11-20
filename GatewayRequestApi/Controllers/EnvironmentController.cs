using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GatewayRequestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        // GET: api/<EnvironmentController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var sqlConn = Environment.GetEnvironmentVariable("SQL_DB_CONNECTION_STRING");
            var appInsConn = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
            var servBusConn = Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING");
            return new string[] { sqlConn, appInsConn, servBusConn };
        }
    }
}
