using Fedelicious_api.Model;
using Fedelicious_api.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;

        public OrderController(IOrderService orderService, IEmailService emailService)
        {
            _orderService = orderService;
            _emailService = emailService;
        }

        // ==========================================
        // 1. GET ALL ORDERS
        // ==========================================
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            return Ok(_orderService.GetAllOrders());
        }

        // ==========================================
        // 2. UPDATE STATUS WITH EMAIL (RESERVATION STYLE)
        // ==========================================
        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateModel model)
        {
            try
            {
                var order = _orderService.GetOrderById(id);
                if (order == null) return NotFound(new { message = "Order not found" });

                // 1. Update sa Database
                order.order_status = model.Status;
                bool success = _orderService.UpdateOrder(order);

                if (success)
                {
                    // 2. Email Logic - Kinopya ang flow mula sa Reservation Controller mo
                    try
                    {
                        string email = _orderService.GetCustomerEmail(order.customer_id);

                        if (!string.IsNullOrEmpty(email))
                        {
                            string subject = $"Fedelicious | Order #{order.order_id} {model.Status}";

                            // Safe check para sa payment_method para hindi mag-error ang string interpolation
                            // Gagamit tayo ng fallback value kung null ang nasa database
                            string pMethod = "To be settled";
                            try { pMethod = order.payment_method ?? "To be settled"; } catch { }

                            string emailBody = $@"
                                <div style='font-family: Arial, sans-serif; max-width: 600px; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                                    <h1 style='color: #ea5b0c; margin-bottom: 20px;'>Order Updated!</h1>
                                    <p style='font-size: 16px; color: #333;'>Hi! Your order <strong>#{order.order_id}</strong> is now <strong>{model.Status}</strong>.</p>
                                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                                    <p style='font-size: 15px;'><strong>Order Type:</strong> {order.order_type}</p>
                                    <p style='font-size: 15px;'><strong>Total Amount:</strong> ₱{order.total_amount:N2}</p>
                                    <p style='font-size: 15px;'><strong>Payment Method:</strong> {pMethod}</p>
                                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #666; font-style: italic;'>Thank you for choosing Fedelicious Wings!</p>
                                </div>";

                            // Ginamit ang generic Send method na working sa reservation mo
                            _emailService.Send(email, subject, emailBody, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Kahit mag-fail ang email, updated na ang status sa DB kaya "Updated!" pa rin ang response
                        System.Diagnostics.Debug.WriteLine("Email Fail: " + ex.Message);
                    }

                    return Ok(new { message = "Updated!" });
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // 3. PLACE ORDER
        // ==========================================
        [HttpPost]
        public IActionResult PlaceOrder([FromBody] Orders newOrder)
        {
            if (newOrder == null) return BadRequest(new { message = "Invalid data" });
            return Ok(_orderService.CreateOrder(newOrder));
        }

        // ==========================================
        // 4. GET ORDERS BY CUSTOMER
        // ==========================================
        [HttpGet("customer/{customerId}")]
        public IActionResult GetCustomerOrders(int customerId)
        {
            return Ok(_orderService.GetOrdersByCustomer(customerId));
        }

        // ==========================================
        // 5. CANCEL/DELETE ORDER
        // ==========================================
        [HttpDelete("{id}")]
        public IActionResult CancelOrder(int id)
        {
            bool isDeleted = _orderService.DeleteOrder(id);
            return isDeleted ? Ok(new { message = "Deleted" }) : BadRequest();
        }

        // ==========================================
        // 6. GET ORDER BY ID
        // ==========================================
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            return order == null ? NotFound() : Ok(order);
        }
    }

    public class StatusUpdateModel
    {
        public string Status { get; set; }
        public int AdminId { get; set; }
    }
}