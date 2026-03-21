using System.Collections.Generic;
using System.Linq;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using MailKit.Search;

namespace Fedelicious_api.Service
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<orders> _orderRepo;
        private readonly IGenericRepository<reservations> _reservationRepo;

        public ReportService(
            IGenericRepository<orders> orderRepo,
            IGenericRepository<reservations> reservationRepo)
        {
            _orderRepo = orderRepo;
            _reservationRepo = reservationRepo;
        }

        public IEnumerable<orders> GetSalesHistory()
        {
            return _orderRepo.GetAll()
                .OrderByDescending(o => o.order_date);
        }

        public IEnumerable<orders> GetOrderReport()
        {
            return _orderRepo.GetAll()
                .OrderByDescending(o => o.order_date);
        }

        public IEnumerable<object> GetRevenueByCategory()
        {
            var orders = _orderRepo.GetAll();

            var result = orders
                .GroupBy(o => o.order_type)
                .Select(g => new
                {
                    Category = string.IsNullOrWhiteSpace(g.Key) ? "Unknown" : g.Key,
                    TotalRevenue = g.Sum(x => x.total_amount)
                });

            return result;
        }

        public decimal GetTotalSales()
        {
            return _orderRepo.GetAll().Sum(o => o.total_amount);
        }

        public int GetTotalOrdersCount()
        {
            return _orderRepo.GetAll().Count();
        }

        public int GetTotalReservations()
        {
            return _reservationRepo.GetAll().Count();
        }
    }
}