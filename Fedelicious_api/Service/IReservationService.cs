using Fedelicious_api.Model;
using System.Collections.Generic;

namespace Fedelicious_api.Service
{
    public interface IReservationService
    {
        IEnumerable<reservations> GetAllReservations();
        reservations GetReservationById(int id);
        IEnumerable<reservations> GetReservationsByCustomer(int customerId);
        reservations CreateReservation(reservations newReservation);
        bool UpdateReservation(reservations updatedReservation);
        bool DeleteReservation(int id);
        string GetCustomerEmail(int customerId);
    }
}