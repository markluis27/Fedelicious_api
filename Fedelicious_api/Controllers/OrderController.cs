using System.Data;
using Dapper;
using Fedelicious_api.Model;
using Fedelicious_api.Service;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly string _connectionString;

        public OrderController(
            IOrderService orderService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _emailService = emailService;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            try
            {
                var orders = _orderService.GetAllOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting orders.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);

                if (order == null)
                    return NotFound(new { message = "Order not found." });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting order.", error = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}")]
        public IActionResult GetCustomerOrders(int customerId)
        {
            try
            {
                var orders = _orderService.GetOrdersByCustomer(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting customer orders.", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult PlaceOrder([FromBody] orders newOrder)
        {
            try
            {
                if (newOrder == null)
                    return BadRequest(new { message = "Invalid order data." });

                var created = _orderService.CreateOrder(newOrder);

                if (created == null)
                    return BadRequest(new { message = "Failed to place order." });

                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error placing order.", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Status))
                {
                    return BadRequest(new { message = "Status is required." });
                }

                var order = _orderService.GetOrderById(id);

                if (order == null)
                {
                    return NotFound(new { message = "Order not found." });
                }

                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Execute(
                        "sp_Orders_UpdateStatus",
                        new
                        {
                            order_id = id,
                            order_status = model.Status,
                            confirmed_by_admin_id = model.AdminId
                        },
                        commandType: CommandType.StoredProcedure
                    );
                }

                try
                {
                    string email = _orderService.GetCustomerEmail(order.customer_id);

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        string subject = $"Fedelicious | Order #{order.order_id} {model.Status}";
                        string paymentMethod = string.IsNullOrWhiteSpace(order.payment_method)
                            ? "To be settled"
                            : order.payment_method;

                        string body = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                                <h1 style='color: #ea5b0c;'>Order Updated</h1>
                                <p>Your order <strong>#{order.order_id}</strong> is now <strong>{model.Status}</strong>.</p>
                                <p><strong>Order Type:</strong> {order.order_type}</p>
                                <p><strong>Total Amount:</strong> ₱{order.total_amount:N2}</p>
                                <p><strong>Payment Method:</strong> {paymentMethod}</p>
                            </div>";

                        _emailService.Send(email, subject, body, true);
                    }
                }
                catch
                {
                }

                return Ok(new { message = "Order status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating order status.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult CancelOrder(int id)
        {
            try
            {
                bool deleted = _orderService.DeleteOrder(id);

                if (!deleted)
                    return BadRequest(new { message = "Failed to delete order." });

                return Ok(new { message = "Order deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting order.", error = ex.Message });
            }
        }
    }

    public class StatusUpdateModel
    {
        public string Status { get; set; }
        public int AdminId { get; set; }
    }
}