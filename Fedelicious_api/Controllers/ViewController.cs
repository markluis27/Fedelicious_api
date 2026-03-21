using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewController : ControllerBase
    {
        private readonly string _connectionString;

        public ViewController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Connection string is NULL!");
            }
        }

        // ===============================
        // 1. ORDER LIST
        // ===============================
        [HttpGet("order-list")]
        public IActionResult GetOrderList()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.Query("SELECT * FROM Order_list");

            return Ok(result);
        }

        // ===============================
        // 2. SOLD ITEMS
        // ===============================
        [HttpGet("sold-items")]
        public IActionResult GetSoldItems()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.Query("SELECT * FROM Sold_Items");

            return Ok(result);
        }

        // ===============================
        // 3. TOTAL USERS
        // ===============================
        [HttpGet("total-users")]
        public IActionResult GetTotalUsers()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.QuerySingleOrDefault("SELECT * FROM totaluser");

            return Ok(result);
        }

        // ===============================
        // 4. TOTAL ORDER
        // ===============================
        [HttpGet("total-orders")]
        public IActionResult GetTotalOrders()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.Query("SELECT * FROM Total_Order");

            return Ok(result);
        }

        // ===============================
        // 5. DAILY REVENUE
        // ===============================
        [HttpGet("daily-revenue")]
        public IActionResult GetDailyRevenue()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.Query("SELECT * FROM Total_Revenue");

            return Ok(result);
        }

        // ===============================
        // 6. AVERAGE MENU
        // ===============================
        [HttpGet("average-menu")]
        public IActionResult GetAverageMenu()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.Query("SELECT * FROM AverageMenu");

            return Ok(result);
        }

        // ===============================
        // 7. DASHBOARD STATS
        // ===============================
        [HttpGet("dashboard")]
        public IActionResult GetDashboardStats()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.QuerySingleOrDefault("SELECT * FROM DashboardStats");

            return Ok(result);
        }

        // ===============================
        // 8. CUSTOMER PAYMENT
        // ===============================
        [HttpGet("customer-payment")]
        public IActionResult GetCustomerPayment()
        {
            using var db = new SqlConnection(_connectionString);

            var result = db.Query("SELECT * FROM Customer_Payment");

            return Ok(result);
        }
    }
}