using Fedelicious_api.Model;
using System.Collections.Generic;

namespace Fedelicious_api.Service
{
    public interface IOrderService
    {
        // Admin - get all orders
        IEnumerable<Orders> GetAllOrders();

        // Customer - view their own orders
        IEnumerable<Orders> GetOrdersByCustomer(int customerId);

        // Get specific order
        Orders GetOrderById(int id);

        // Place order - Ngayon ay nag-re-return na ng mismong object
        Orders CreateOrder(Orders newOrder);

        // Update order status
        bool UpdateOrder(Orders updatedOrder);

        // Cancel order
        bool DeleteOrder(int id);
        string GetCustomerEmail(int customer_id);
    }
}