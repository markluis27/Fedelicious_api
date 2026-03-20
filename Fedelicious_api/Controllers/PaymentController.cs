using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IGenericRepository<payments> _paymentRepo;
        private readonly IGenericRepository<Orders> _orderRepo;
        private readonly IGenericRepository<reservations> _reservationRepo;

        public PaymentController(
            IGenericRepository<payments> paymentRepo,
            IGenericRepository<Orders> orderRepo,
            IGenericRepository<reservations> reservationRepo)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _reservationRepo = reservationRepo;
        }

        [HttpPost]
        public IActionResult ProcessPayment([FromBody] payments newPayment)
        {
            try
            {
                if (newPayment == null) return BadRequest("No data received.");

                
                int? resId = newPayment.reservation_id;
                int? ordId = newPayment.order_id;

                newPayment.reservation_id = resId;
                newPayment.order_id = ordId;

                if (newPayment.reservation_id == null && newPayment.order_id == null)
                {
                    return BadRequest(new { message = "Kailangan ng Reservation ID o Order ID." });
                }

                newPayment.payment_date = DateTime.Now;
                newPayment.payment_status = "Pending Verification";

                bool saved = _paymentRepo.Add(newPayment);

                if (saved)
                {
                    return Ok(new
                    {
                        message = "Success! Payment saved.",
                        saved_reservation_id = newPayment.reservation_id,
                        saved_order_id = newPayment.order_id
                    });
                }

                return BadRequest(new { message = "Database Error: Make sure the ID exists in Reservations/Orders table." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}