using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using MailKit.Search;

namespace Fedelicious_api.Service
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<orders> _orderRepo;
        private readonly IGenericRepository<customers> _customerRepo;

        public OrderService(
            IGenericRepository<orders> orderRepo,
            IGenericRepository<customers> customerRepo)
        {
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
        }

        public IEnumerable<orders> GetAllOrders()
        {
            return _orderRepo.GetAll();
        }

        public IEnumerable<orders> GetOrdersByCustomer(int customerId)
        {
            return _orderRepo.GetAll().Where(o => o.customer_id == customerId);
        }

        public orders GetOrderById(int id)
        {
            return _orderRepo.GetById(id);
        }

        public orders CreateOrder(orders newOrder)
        {
            if (newOrder == null) return null;

            if (string.IsNullOrWhiteSpace(newOrder.order_status))
                newOrder.order_status = "Pending";

            if (string.IsNullOrWhiteSpace(newOrder.payment_status))
                newOrder.payment_status = "Pending";

            if (newOrder.order_date == default)
                newOrder.order_date = DateTime.Now;

            bool added = _orderRepo.Add(newOrder);
            return added ? newOrder : null;
        }

        public bool UpdateOrder(orders updatedOrder)
        {
            if (updatedOrder == null) return false;
            return _orderRepo.Update(updatedOrder);
        }

        public bool DeleteOrder(int id)
        {
            return _orderRepo.Delete(id);
        }

        public string GetCustomerEmail(int customer_id)
        {
            var customer = _customerRepo.GetById(customer_id);
            return customer?.email ?? "";
        }
    }
}