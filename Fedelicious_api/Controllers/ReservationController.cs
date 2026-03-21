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

        [HttpPost]
        public IActionResult CreateReservation([FromBody] reservations newReservation)
        {
            try
            {
                if (newReservation == null)
                    return BadRequest(new { message = "Invalid reservation data." });

                var created = _reservationService.CreateReservation(newReservation);

                if (created == null)
                    return BadRequest(new { message = "Failed to create reservation." });

                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating reservation.", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllReservations()
        {
            try
            {
                var data = _reservationService.GetAllReservations();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting reservations.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetReservationById(int id)
        {
            try
            {
                var item = _reservationService.GetReservationById(id);

                if (item == null)
                    return NotFound(new { message = "Reservation not found." });

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting reservation.", error = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}")]
        public IActionResult GetCustomerReservations(int customerId)
        {
            try
            {
                var reservations = _reservationService.GetReservationsByCustomer(customerId);
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting customer reservations.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, [FromBody] reservations updatedReservation)
        {
            try
            {
                if (updatedReservation == null)
                    return BadRequest(new { message = "Invalid reservation data." });

                updatedReservation.reservation_id = id;

                bool updated = _reservationService.UpdateReservation(updatedReservation);

                if (!updated)
                    return BadRequest(new { message = "Failed to update reservation." });

                return Ok(new { message = "Reservation updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating reservation.", error = ex.Message });
            }
        }

        [HttpPut("confirm/{id}")]
        public IActionResult ConfirmReservation(int id)
        {
            try
            {
                var reservation = _reservationService.GetReservationById(id);

                if (reservation == null)
                    return NotFound(new { message = "Reservation not found." });

                reservation.reservation_status = "Confirmed";

                bool updated = _reservationService.UpdateReservation(reservation);

                if (!updated)
                    return BadRequest(new { message = "Failed to confirm reservation." });

                try
                {
                    string email = _reservationService.GetCustomerEmail(reservation.customer_id);

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        string subject = $"Fedelicious | Reservation #{reservation.reservation_id} Confirmed";
                        string body = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                                <h1 style='color: #ea5b0c;'>Reservation Confirmed</h1>
                                <p>Your reservation <strong>#{reservation.reservation_id}</strong> has been confirmed.</p>
                                <p><strong>Date:</strong> {reservation.reservation_date:MMMM dd, yyyy}</p>
                                <p><strong>Time:</strong> {reservation.reservation_time}</p>
                                <p><strong>Guests:</strong> {reservation.number_of_guests}</p>
                            </div>";

                        _emailService.Send(email, subject, body, true);
                    }
                }
                catch
                {
                }

                return Ok(new { message = "Reservation confirmed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error confirming reservation.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult CancelReservation(int id)
        {
            try
            {
                bool deleted = _reservationService.DeleteReservation(id);

                if (!deleted)
                    return BadRequest(new { message = "Failed to cancel reservation." });

                return Ok(new { message = "Reservation cancelled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting reservation.", error = ex.Message });
            }
        }
    }
}