using System.Collections.Generic;
using Fedelicious_api.Model;

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