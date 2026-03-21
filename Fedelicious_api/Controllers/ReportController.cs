using System.Data;
using Dapper;
using Fedelicious_api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var result = await db.QueryFirstOrDefaultAsync(
                    "SP_totalrevenvue",
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                    return NotFound(new { message = "No dashboard summary found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting dashboard summary.", error = ex.Message });
            }
        }

        [HttpGet("sales")]
        public IActionResult GetSalesReport()
        {
            try
            {
                var sales = _reportService.GetSalesHistory();

                return Ok(new
                {
                    message = "Sales report generated successfully.",
                    data = sales
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting sales report.", error = ex.Message });
            }
        }

        [HttpGet("orders")]
        public IActionResult GetOrderReport()
        {
            try
            {
                var orders = _reportService.GetOrderReport();

                return Ok(new
                {
                    message = "Order report retrieved successfully.",
                    data = orders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting order report.", error = ex.Message });
            }
        }

        [HttpGet("category-revenue")]
        public IActionResult GetRevenueByCategory()
        {
            try
            {
                var revenue = _reportService.GetRevenueByCategory();

                return Ok(new
                {
                    message = "Revenue by category retrieved successfully.",
                    data = revenue
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting category revenue.", error = ex.Message });
            }
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboardStats()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting dashboard stats.", error = ex.Message });
            }
        }
    }
}