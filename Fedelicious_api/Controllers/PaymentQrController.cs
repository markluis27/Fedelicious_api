using System;
using System.Linq; // Idagdag ito kung gagamit ng .Any()
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentQrController : ControllerBase
    {
        private readonly IGenericRepository<payment_qr_settings> _qrRepo;

        public PaymentQrController(IGenericRepository<payment_qr_settings> qrRepo)
        {
            _qrRepo = qrRepo;
        }

        // ==========================================
        // 1. ADD QR (ADMIN)
        // POST: api/paymentqr
        // ==========================================
        [HttpPost]
        public IActionResult AddPaymentQr([FromBody] payment_qr_settings newQr)
        {
            if (newQr == null)
                return BadRequest(new { message = "Invalid QR data." });

            // Set defaults
            newQr.is_active = true;
            newQr.updated_at = DateTime.Now;

            try
            {
                // Gamitin ang connection string mo
                string connectionString = "Server=LAPTOP-OU71PFMJ\\SQLEXPRESS;Database=Fedelicious;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
                using var connection = new SqlConnection(connectionString);

                // Tawagin ang Stored Procedure gamit ang Dapper
                int newId = connection.ExecuteScalar<int>(
                    "sp_InsertPaymentQr",
                    new
                    {
                        qr_name = newQr.qr_name,
                        qr_image = newQr.qr_image,
                        is_active = newQr.is_active,
                        updated_at = newQr.updated_at
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (newId > 0)
                {
                    return Ok(new
                    {
                        message = "Payment QR added successfully via Stored Procedure!",
                        paymentqr_id = newId
                    });
                }

                return BadRequest(new { message = "Failed to add QR. The Stored Procedure returned 0." });
            }
            catch (Exception ex)
            {
                // Kapag may error sa SP (tulad ng missing table o wrong data type), lalabas agad dito.
                return StatusCode(500, new
                {
                    message = "A database error occurred.",
                    errorDetail = ex.Message
                });
            }
        }

        // ==========================================
        // 2. GET ALL QRS (ADMIN MANAGEMENT)
        // GET: api/paymentqr
        // ==========================================
        [HttpGet]
        public IActionResult GetAllPaymentQrs()
        {
            try
            {
                var qrList = _qrRepo.GetAll();

                if (qrList == null || !qrList.Any())
                {
                    return NotFound(new { message = "No payment QR codes found." });
                }

                return Ok(qrList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the QR codes.",
                    errorDetail = ex.Message
                });
            }
        }

        // ==========================================
        // 3. GET ACTIVE QR (PARA SA CHECKOUT NG CUSTOMER)
        // GET: api/paymentqr/active
        // ==========================================
        [HttpGet("active")]
        public IActionResult GetActivePaymentQrs()
        {
            try
            {
                // Kukunin lang yung mga may is_active = true
                var activeQrs = _qrRepo.GetAll().Where(q => q.is_active == true).ToList();

                if (activeQrs == null || !activeQrs.Any())
                {
                    return NotFound(new { message = "No active payment QR codes found." });
                }

                return Ok(activeQrs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving active QR codes.",
                    errorDetail = ex.Message
                });
            }
        }

        // ==========================================
        // 4. DELETE QR (ADMIN)
        // DELETE: api/paymentqr/{id}
        // ==========================================
        [HttpDelete("{id}")]
        public IActionResult DeletePaymentQr(int id)
        {
            bool isDeleted = _qrRepo.Delete(id);

            if (!isDeleted)
                return BadRequest(new { message = "Failed to delete Payment QR. ID might not exist." });

            return Ok(new { message = "Payment QR deleted successfully!" });
        }
        [HttpPut("set-active/{id}")]
        public IActionResult SetActiveQr(int id)
        {
            try
            {
                // Kunin muna ang QR code para malaman kung GCash o Maya ito
                var targetQr = _qrRepo.GetById(id);
                if (targetQr == null)
                    return NotFound(new { message = "QR code not found." });

                string connectionString = "Server=LAPTOP-OU71PFMJ\\SQLEXPRESS;Database=Fedelicious;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
                using var connection = new SqlConnection(connectionString);

                // Patayin muna (is_active = 0) lahat ng QR na may kaparehong qr_name (e.g. patayin lahat ng GCash)
                connection.Execute(
                    "UPDATE payment_qr_settings SET is_active = 0 WHERE qr_name = @Name",
                    new { Name = targetQr.qr_name }
                );

                // Buhayin (is_active = 1) ang mismong QR na pinindot ng Admin
                connection.Execute(
                    "UPDATE payment_qr_settings SET is_active = 1 WHERE paymentqr_id = @Id",
                    new { Id = id }
                );

                return Ok(new { message = $"{targetQr.qr_name} QR is now active and ready for customers!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database error.", errorDetail = ex.Message });
            }
        }
    }
}