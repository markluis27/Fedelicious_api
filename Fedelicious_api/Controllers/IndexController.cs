using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly string _connectionString;

        public IndexController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Connection string is NULL!");
            }
        }

        // ===============================
        // DASHBOARD SUMMARY
        // ===============================
        [HttpGet]
        public IActionResult GetDashboardSummary()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.QuerySingleOrDefault("SELECT * FROM DashboardStats");

            return Ok(result);
        }
    }
}