using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Fedelicious_api.Service
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<Orders> _orderRepo;
        private readonly IGenericRepository<reservations> _reservationRepo;

        public ReportService(
            IGenericRepository<Orders> orderRepo,
            IGenericRepository<reservations> reservationRepo)
        {
            _orderRepo = orderRepo;
            _reservationRepo = reservationRepo;
        }

        // ==================================
        // SALES HISTORY
        // ==================================
        public IEnumerable<Orders> GetSalesHistory()
        {
            return _orderRepo.GetAll();
        }

        // ==================================
        // ORDER REPORT
        // ==================================
        public IEnumerable<Orders> GetOrderReport()
        {
            return _orderRepo.GetAll();
        }

        // ==================================
        // REVENUE BY CATEGORY
        // ==================================
        public IEnumerable<object> GetRevenueByCategory()
        {
            var orders = _orderRepo.GetAll();

            var revenue = orders
                .GroupBy(o => o.order_type)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalRevenue = g.Sum(x => x.total_amount)
                });

            return revenue;
        }

        // ==================================
        // TOTAL SALES
        // ==================================
        public decimal GetTotalSales()
        {
            var orders = _orderRepo.GetAll();

            return orders.Sum(o => o.total_amount);
        }

        // ==================================
        // TOTAL ORDERS
        // ==================================
        public int GetTotalOrdersCount()
        {
            return _orderRepo.GetAll().Count();
        }

        // ==================================
        // TOTAL RESERVATIONS
        // ==================================
        public int GetTotalReservations()
        {
            return _reservationRepo.GetAll().Count();
        }
    }
}