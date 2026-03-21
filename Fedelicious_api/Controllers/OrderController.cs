using System.Data;
using Dapper;
using Fedelicious_api.Model;
using Fedelicious_api.Service;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Linq;

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
                        string subject;
                        string body;

                        string paymentMethod = string.IsNullOrWhiteSpace(order.payment_method)
                            ? "To be settled"
                            : order.payment_method;

                        // ✅ SAFE FETCH ITEMS
                        string orderItemsHtml = "<p>No item details available.</p>";

                        try
                        {
                            using (IDbConnection db = new SqlConnection(_connectionString))
                            {
                                var items = db.Query(@"
									SELECT m.item_name, oi.quantity
									FROM order_items oi
									LEFT JOIN menu_items m ON oi.menu_item_id = m.menu_item_id
									WHERE oi.order_id = @orderId
								", new { orderId = id }).ToList();

                                if (items != null && items.Any())
                                {
                                    orderItemsHtml = "<ul style='text-align:left; padding-left:20px;'>";
                                    foreach (var item in items)
                                    {
                                        orderItemsHtml += $"<li>{item.item_name} (x{item.quantity})</li>";
                                    }
                                    orderItemsHtml += "</ul>";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ITEM FETCH ERROR: " + ex.Message);
                        }

                        // 🔴 PREMIUM CANCELLED EMAIL
                        if (model.Status == "Cancelled")
                        {
                            subject = $"Fedelicious | ORDER CANCELLED #{order.order_id}";

                            body = $@"
							<div style='font-family: Poppins, Arial, sans-serif; background:#f4f6f8; padding:30px;'>

								<div style='max-width:600px; margin:auto; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 8px 25px rgba(0,0,0,0.08);'>

									<div style='background:#e74c3c; padding:20px; text-align:center; color:white;'>
										<h1 style='margin:0; font-size:28px;'>ORDER CANCELLED</h1>
										<p style='margin:5px 0 0 0;'>Fedelicious Notification</p>
									</div>

									<div style='padding:25px;'>

										<p>Hello,</p>

										<p>Your order <strong>#{order.order_id}</strong> has been cancelled.</p>

										<div style='background:#fff3f3; border-left:5px solid #e74c3c; padding:15px; border-radius:8px; margin:20px 0;'>
											<strong>Reason:</strong>
											<p>{model.Reason ?? "No reason provided"}</p>
										</div>

										<h3>Items Ordered</h3>
										<div style='background:#fafafa; padding:15px; border-radius:8px; border:1px solid #eee;'>
											{orderItemsHtml}
										</div>

										<p><strong>Order Type:</strong> {order.order_type}</p>
										<p><strong>Total Amount:</strong> ₱{order.total_amount:N2}</p>

									</div>

									<div style='background:#f9f9f9; padding:15px; text-align:center; font-size:12px; color:#999;'>
										<p>If you have questions, contact support.</p>
										<p>© Fedelicious</p>
									</div>

								</div>

							</div>";
                        }
                        else
                        {
                            subject = $"Fedelicious | Order #{order.order_id} {model.Status}";

                            body = $@"
							<div style='font-family: Arial; max-width:600px; border:1px solid #eee; padding:20px; border-radius:10px;'>

								<h1 style='color:#ea5b0c;'>Order Updated</h1>

								<p>Your order <strong>#{order.order_id}</strong> is now <strong>{model.Status}</strong>.</p>

								<h3>Items Ordered:</h3>
								{orderItemsHtml}

								<p><strong>Order Type:</strong> {order.order_type}</p>
								<p><strong>Total Amount:</strong> ₱{order.total_amount:N2}</p>
								<p><strong>Payment Method:</strong> {paymentMethod}</p>

							</div>";
                        }

                        _emailService.Send(email, subject, body, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EMAIL ERROR: " + ex.Message);
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
        public string? Reason { get; set; }
    }
}
