using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Fedelicious_api.Service
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Orders> _orderRepo;
        // 1. Idagdag ang Customer Repository para makuha ang email (Gaya ng sa Reservation)
        private readonly IGenericRepository<customers> _customerRepo;

        // 2. I-update ang constructor para tanggapin ang customerRepo
        public OrderService(
            IGenericRepository<Orders> orderRepo,
            IGenericRepository<customers> customerRepo)
        {
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
        }

        // ==================================
        // GET CUSTOMER EMAIL (EKSAKTONG GAYA NG SA RESERVATION)
        // ==================================
        public string GetCustomerEmail(int customerId)
        {
            // Gagamit na tayo ng repo imbes na direct SQL query para pareho sila ng logic
            var customer = _customerRepo.GetById(customerId);
            return customer?.email ?? "";
        }

        // ==================================
        // GET ALL ORDERS (ADMIN)
        // ==================================
        public IEnumerable<Orders> GetAllOrders()
        {
            return _orderRepo.GetAll();
        }

        // ==================================
        // GET ORDERS BY CUSTOMER
        // ==================================
        public IEnumerable<Orders> GetOrdersByCustomer(int customerId)
        {
            return _orderRepo.GetAll().Where(o => o.customer_id == customerId);
        }

        // ==================================
        // GET ORDER BY ID
        // ==================================
        public Orders GetOrderById(int id)
        {
            return _orderRepo.GetById(id);
        }

        // ==================================
        // CREATE ORDER
        // ==================================
        public Orders CreateOrder(Orders newOrder)
        {
            if (newOrder == null) return null;
            newOrder.order_status = "Pending";
            bool isAdded = _orderRepo.Add(newOrder);
            return isAdded ? newOrder : null;
        }

        // ==================================
        // UPDATE ORDER
        // ==================================
        public bool UpdateOrder(Orders updatedOrder)
        {
            if (updatedOrder == null) return false;
            return _orderRepo.Update(updatedOrder);
        }

        // ==================================
        // DELETE / CANCEL ORDER
        // ==================================
        public bool DeleteOrder(int id)
        {
            return _orderRepo.Delete(id);
        }
    }
}