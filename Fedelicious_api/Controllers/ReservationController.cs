using System;
using System.Linq;
using Fedelicious_api.Model;
using Fedelicious_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IEmailService _emailService;

        public ReservationController(IReservationService reservationService, IEmailService emailService)
        {
            _reservationService = reservationService;
            _emailService = emailService;
        }

        // ==========================================
        // PARA SA CUSTOMER HISTORY (orders.html)
        // GET: api/Reservation/customer/{customerId}
        // ==========================================
        [HttpGet("customer/{customerId}")]
        public IActionResult GetCustomerHistory(int customerId)
        {
            try
            {
                var history = _reservationService.GetReservationsByCustomer(customerId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Reservation (Para sa Admin)
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_reservationService.GetAllReservations());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("confirm/{id}")]
        public IActionResult ConfirmReservation(int id)
        {
            try
            {
                var reservation = _reservationService.GetReservationById(id);
                if (reservation == null) return NotFound();

                reservation.reservation_status = "Confirmed";
                bool success = _reservationService.UpdateReservation(reservation);

                if (success)
                {
                    try
                    {
                        string email = _reservationService.GetCustomerEmail(reservation.customer_id);
                        if (!string.IsNullOrEmpty(email))
                        {
                            string emailBody = $@"
                                <div style='font-family: Arial, sans-serif; max-width: 600px; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                                    <h1 style='color: #ea5b0c; margin-bottom: 20px;'>Booking Confirmed!</h1>
                                    <p style='font-size: 16px; color: #333;'>Hi! Your reservation <strong>#{reservation.reservation_id}</strong> has been verified and confirmed.</p>
                                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                                    <p style='font-size: 16px; margin: 10px 0;'><strong>Date:</strong> {reservation.reservation_date:MM/dd/yyyy}</p>
                                    <p style='font-size: 16px; margin: 10px 0;'><strong>Time:</strong> {reservation.reservation_time}</p>
                                    <p style='font-size: 16px; margin: 10px 0;'><strong>Guests:</strong> {reservation.number_of_guests} Pax</p>
                                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #666; font-style: italic;'>See you at Fedelicious Wings! Please arrive 15 minutes before your schedule.</p>
                                </div>";

                            _emailService.Send(email, "Fedelicious | Confirmed!", emailBody, true);
                        }
                    }
                    catch { }
                    return Ok(new { message = "Confirmed!" });
                }
                return BadRequest();
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok(_reservationService.GetReservationById(id));

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) => Ok(_reservationService.DeleteReservation(id));

        [HttpPost]
        public IActionResult Create([FromBody] reservations model) => Ok(_reservationService.CreateReservation(model));
    }
}