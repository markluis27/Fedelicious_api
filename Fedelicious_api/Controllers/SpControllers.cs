using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpController : ControllerBase
    {
        private readonly string _connectionString =
            "Server=LAPTOP-OU71PFMJ\\SQLEXPRESS;Database=Fedelicious;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

        // ==========================================
        // SECTION 1: ORIGINAL ENDPOINTS (DATI MONG GAWA)
        // ==========================================

        [HttpGet("reservations")]
        public async Task<IActionResult> GetReservations()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await db.QueryAsync<dynamic>("sp_GetReservationIntelligence", commandType: CommandType.StoredProcedure);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Reservation SP Error: " + ex.Message });
                }
            }
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await db.QueryAsync<dynamic>("sp_GetOrder", commandType: CommandType.StoredProcedure);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Order SP Error: " + ex.Message });
                }
            }
        }

        // ==========================================
        // SECTION 2: DASHBOARD (STAT CARDS)
        // ==========================================

        // SP 4 - Total Users
        [HttpGet("dashboard/total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryFirstOrDefaultAsync<dynamic>("SP_totaluser", commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 9 - Daily Revenue
        [HttpGet("dashboard/daily-revenue")]
        public async Task<IActionResult> GetDailyRevenue()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_dailyrevenue", commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 11 - Count Reservations by Date
        [HttpGet("dashboard/count-reservations")]
        public async Task<IActionResult> GetCountReservations([FromQuery] string date)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryFirstOrDefaultAsync<dynamic>("SP_countreservation", new { reservation_date = date }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 15 - Weekly and Daily Sales Summary
        [HttpGet("dashboard/grand-overview")]
        public async Task<IActionResult> GetGrandOverview()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync<dynamic>("SP_GrandDashboardStats", commandType: CommandType.StoredProcedure);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "SQL Error: " + ex.Message });
                }
            }
        }

        // ==========================================
        // SECTION 3: SALES REPORT (ANALYTICS)
        // ==========================================

        // SP 12 - Yearly Summary
        [HttpGet("sales/yearly-summary")]
        public async Task<IActionResult> GetYearlySummary([FromQuery] int year)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryFirstOrDefaultAsync<dynamic>("SP_yearlysummary", new { year = year }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 7 - Monthly Sales History
        [HttpGet("sales/monthly-history")]
        public async Task<IActionResult> GetMonthlyHistory([FromQuery] int year)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_monthsales", new { TargetYear = year }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 5 - Weekly Sales Report (Filtered)
        [HttpGet("sales/weekly-report")]
        public async Task<IActionResult> GetWeeklyReport([FromQuery] int month, [FromQuery] int year)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_weeklysalesreport", new { TargetMonth = month, TargetYear = year }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 2 - Best Sellers (Total Sold)
        [HttpGet("sales/best-sellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_countotalsold", commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 14 - Above Average Menu Items
        [HttpGet("sales/above-average")]
        public async Task<IActionResult> GetAboveAverage()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_aboveaveragemenu", commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 6 - Revenue by Order Type
        [HttpGet("sales/revenue-type")]
        public async Task<IActionResult> GetRevenueType()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_insertrevenue", commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 3 - Sales Between Dates
        [HttpGet("sales/filter-date")]
        public async Task<IActionResult> GetSalesBetween([FromQuery] string start, [FromQuery] string end)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_betweenorder", new { StartDate = start, EndDate = end }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // ==========================================
        // SECTION 4: CUSTOMER ACTIVITY (HISTORY)
        // ==========================================

        // SP 1 - Customer Pioneer Stats (Min/Max Order)
        [HttpGet("activity/pioneer-stats")]
        public async Task<IActionResult> GetPioneerStats()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_minandmax", commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 8 - Reservation Summary (Min/Max Guests)
        [HttpGet("activity/summary")]
        public async Task<IActionResult> GetCustSummary([FromQuery] string name)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_summaryorders", new { fullname = name }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 10 - Detailed Reservation List
        [HttpGet("activity/list-reservations")]
        public async Task<IActionResult> GetCustList([FromQuery] string name)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_Listreservation", new { fullname = name }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 13 - Full Customer Profile (Subqueries)
        [HttpGet("activity/full-profile")]
        public async Task<IActionResult> GetFullProfile([FromQuery] string name = null)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_getCustomerReservations", new { fullname = name }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        // SP 16 - Payment Settlement Report
        [HttpGet("activity/payment-report")]
        public async Task<IActionResult> GetPaymentReport([FromQuery] string name)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<dynamic>("SP_customerpaymentreport", new { fullname = name }, commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }

    }
}