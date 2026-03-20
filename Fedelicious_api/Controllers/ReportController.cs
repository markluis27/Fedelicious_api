using System.Data;
using Dapper;
using Fedelicious_api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly string _connectionString;

        public ReportController(IReportService reportService, IConfiguration configuration)
        {
            _reportService = reportService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ==========================================
        // CALL STORED PROCEDURE: SP_totalrevenvue
        // GET: api/Report/dashboard-summary
        // ==========================================
        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            // Pag-check kung nakuha ba ang connection string
            if (string.IsNullOrEmpty(_connectionString))
            {
                return StatusCode(500, new { message = "Connection string 'DefaultConnection' is missing." });
            }

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync("SP_totalrevenvue",
                                 commandType: CommandType.StoredProcedure);

                    if (result != null)
                    {
                        return Ok(result);
                    }
                    return NotFound(new { message = "Walang data na ibinalik ang Stored Procedure." });
                }
                catch (System.Exception ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
        }

        // ==========================================
        // 1. GENERATE SALES REPORT
        // GET: api/report/sales
        // ==========================================
        [HttpGet("sales")]
        public IActionResult GetSalesReport()
        {
            var sales = _reportService.GetSalesHistory();
            return Ok(new
            {
                message = "Sales report generated successfully.",
                data = sales
            });
        }

        // ==========================================
        // 2. GENERATE ORDER REPORT
        // GET: api/report/orders
        // ==========================================
        [HttpGet("orders")]
        public IActionResult GetOrderReport()
        {
            var orders = _reportService.GetOrderReport();
            return Ok(new
            {
                message = "Order report retrieved successfully.",
                data = orders
            });
        }

        // ==========================================
        // 3. REVENUE BY CATEGORY
        // GET: api/report/category-revenue
        // ==========================================
        [HttpGet("category-revenue")]
        public IActionResult GetRevenueByCategory()
        {
            var revenue = _reportService.GetRevenueByCategory();
            return Ok(new
            {
                message = "Revenue by category retrieved successfully.",
                data = revenue
            });
        }

        // ==========================================
        // 4. DASHBOARD SUMMARY (VIA SERVICE)
        // GET: api/report/dashboard
        // ==========================================
        [HttpGet("dashboard")]
        public IActionResult GetDashboardStats()
        {
            var totalSales = _reportService.GetTotalSales();
            var totalOrders = _reportService.GetTotalOrdersCount();
            var totalReservations = _reportService.GetTotalReservations();

            return Ok(new
            {
                message = "Dashboard statistics retrieved successfully.",
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                TotalReservations = totalReservations
            });
        }
    }
}