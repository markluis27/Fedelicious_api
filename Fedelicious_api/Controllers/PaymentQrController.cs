using System.Data;
using Dapper;
using Fedelicious_api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentQrController : ControllerBase
    {
        private readonly string _connectionString;

        public PaymentQrController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ==========================================
        // DTO for upload (IMPORTANT)
        // ==========================================
        public class PaymentQrUploadModel
        {
            public string qr_name { get; set; }
            public string qr_accname { get; set; }
            public IFormFile qr_image { get; set; }
            public bool is_active { get; set; }
        }

        // ==========================================
        // ADD QR
        // ==========================================
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddPaymentQr([FromForm] PaymentQrUploadModel newQr)
        {
            try
            {
                if (newQr == null ||
                    string.IsNullOrWhiteSpace(newQr.qr_name) ||
                    string.IsNullOrWhiteSpace(newQr.qr_accname))
                {
                    return BadRequest(new { message = "QR name and account name are required." });
                }

                if (newQr.qr_image == null || newQr.qr_image.Length == 0)
                {
                    return BadRequest(new { message = "QR image is required." });
                }

                byte[] imageBytes;

                using (var ms = new MemoryStream())
                {
                    await newQr.qr_image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                using IDbConnection db = new SqlConnection(_connectionString);

                int newId = db.ExecuteScalar<int>(
                    "sp_PaymentQr_Create",
                    new
                    {
                        qr_name = newQr.qr_name,
                        qr_accname = newQr.qr_accname,
                        qr_image = imageBytes,
                        is_active = newQr.is_active
                    },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new
                {
                    message = "QR added successfully.",
                    paymentqr_id = newId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error saving QR.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // GET ALL QR
        // ==========================================
        [HttpGet]
        public IActionResult GetAllPaymentQrs()
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var qrs = db.Query<dynamic>(
                    "sp_PaymentQr_GetAll",
                    commandType: CommandType.StoredProcedure
                ).Select(q => new
                {
                    paymentqr_id = q.paymentqr_id,
                    qr_name = q.qr_name,
                    qr_accname = q.qr_accname,
                    qr_image = q.qr_image != null ? Convert.ToBase64String((byte[])q.qr_image) : "",
                    is_active = q.is_active,
                    updated_at = q.updated_at
                });

                return Ok(qrs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error loading QR.", error = ex.Message });
            }
        }

        // ==========================================
        // GET ACTIVE
        // ==========================================
        [HttpGet("active")]
        public IActionResult GetActivePaymentQrs()
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var qrs = db.Query<dynamic>(
                    "sp_PaymentQr_GetActive",
                    commandType: CommandType.StoredProcedure
                ).Select(q => new
                {
                    paymentqr_id = q.paymentqr_id,
                    qr_name = q.qr_name,
                    qr_accname = q.qr_accname,
                    qr_image = q.qr_image != null ? Convert.ToBase64String((byte[])q.qr_image) : "",
                    is_active = q.is_active
                });

                return Ok(qrs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error loading active QR.", error = ex.Message });
            }
        }

        // ==========================================
        // DELETE
        // ==========================================
        [HttpDelete("{id}")]
        public IActionResult DeletePaymentQr(int id)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                db.Execute(
                    "sp_PaymentQr_Delete",
                    new { paymentqr_id = id },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new { message = "QR deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting QR.", error = ex.Message });
            }
        }

        // ==========================================
        // SET ACTIVE
        // ==========================================
        [HttpPut("set-active/{id}")]
        public IActionResult SetActiveQr(int id)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                db.Execute(
                    "sp_PaymentQr_SetActive",
                    new { paymentqr_id = id },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new { message = "QR set as active." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error setting active QR.", error = ex.Message });
            }
        }
    }
}