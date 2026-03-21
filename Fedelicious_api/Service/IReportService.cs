using System.Collections.Generic;
using Fedelicious_api.Model;
using MailKit.Search;

namespace Fedelicious_api.Service
{
    public interface IReportService
    {
        IEnumerable<orders> GetSalesHistory();
        IEnumerable<orders> GetOrderReport();
        IEnumerable<object> GetRevenueByCategory();

        decimal GetTotalSales();
        int GetTotalOrdersCount();
        int GetTotalReservations();
    }
}