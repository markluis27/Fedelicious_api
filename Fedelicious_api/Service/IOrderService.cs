using Fedelicious_api.Model;
using System.Collections.Generic;

namespace Fedelicious_api.Service
{
    public interface IOrderService
    {
        // Admin - get all orders
        IEnumerable<orders> GetAllOrders();

        // Customer - view their own orders
        IEnumerable<orders> GetOrdersByCustomer(int customerId);

        // Get specific order
        orders GetOrderById(int id);

        // Place order - Ngayon ay nag-re-return na ng mismong object
        orders CreateOrder(orders newOrder);

        // Update order status
        bool UpdateOrder(orders updatedOrder);

        // Cancel order
        bool DeleteOrder(int id);
        string GetCustomerEmail(int customer_id);
    }
}