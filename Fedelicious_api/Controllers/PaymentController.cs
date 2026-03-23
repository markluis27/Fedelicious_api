using System;
using Fedelicious_api.Model;
using Fedelicious_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public IActionResult ProcessPayment([FromBody] payments newPayment)
        {
            try
            {
                if (newPayment == null)
                {
                    return BadRequest(new { message = "No data received." });
                }

                if (newPayment.order_id == null && newPayment.reservation_id == null)
                {
                    return BadRequest(new { message = "Order ID or Reservation ID is required." });
                }

                if (string.IsNullOrWhiteSpace(newPayment.reference_number))
                {
                    return BadRequest(new { message = "Reference number is required." });
                }

                if (newPayment.amount <= 0)
                {
                    return BadRequest(new { message = "Amount must be greater than zero." });
                }

                if (string.IsNullOrWhiteSpace(newPayment.payment_method))
                {
                    newPayment.payment_method = "GCash";
                }

                if (newPayment.paymentqr_id == null || newPayment.paymentqr_id <= 0)
                {
                    return BadRequest(new { message = "Payment QR ID is required." });
                }

                newPayment.payment_date = DateTime.Now;
                newPayment.payment_status = "Pending Verification";

                bool saved = _paymentService.SubmitGcashPayment(newPayment);

                if (!saved)
                {
                    return BadRequest(new { message = "Failed to save payment." });
                }

                return Ok(new
                {
                    message = "Payment saved successfully.",
                    payment_id = newPayment.payment_id,
                    saved_order_id = newPayment.order_id,
                    saved_reservation_id = newPayment.reservation_id,
                    payment_method = newPayment.payment_method,
                    payment_status = newPayment.payment_status,
                    payment_date = newPayment.payment_date,
                    paymentqr_id = newPayment.paymentqr_id
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "REFERENCE_EXISTS")
                {
                    return BadRequest(new { message = "Reference number already exists." });
                }

                if (ex.Message == "PAYMENT_INSERT_FAILED")
                {
                    return BadRequest(new { message = "Payment insert failed. Check database columns, foreign keys, or required fields." });
                }

                return StatusCode(500, new
                {
                    message = "Error processing payment.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("order/{orderId}")]
        public IActionResult GetPaymentByOrderId(int orderId)
        {
            try
            {
                var payment = _paymentService.GetPaymentByOrderId(orderId);

                if (payment == null)
                {
                    return NotFound(new { message = "No payment found for this order." });
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error getting payment by order.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("reservation/{reservationId}")]
        public IActionResult GetPaymentByReservationId(int reservationId)
        {
            try
            {
                var payment = _paymentService.GetPaymentByReservationId(reservationId);

                if (payment == null)
                {
                    return NotFound(new { message = "No payment found for this reservation." });
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error getting payment by reservation.",
                    error = ex.Message
                });
            }
        }
    }
}