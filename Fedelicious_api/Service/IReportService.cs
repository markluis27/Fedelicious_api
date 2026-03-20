using Fedelicious_api.Model;
using System.Collections.Generic;

namespace Fedelicious_api.Service
{
    public interface IReportService
    {
        // ==================================
        // SALES REPORT
        // ==================================
        IEnumerable<Orders> GetSalesHistory();

        // ==================================
        // ORDER REPORT
        // ==================================
        IEnumerable<Orders> GetOrderReport();

        // ==================================
        // CATEGORY REVENUE
        // ==================================
        IEnumerable<object> GetRevenueByCategory();

        // ==================================
        // DASHBOARD STATS
        // ==================================
        decimal GetTotalSales();

        int GetTotalOrdersCount();

        int GetTotalReservations();
    }
}