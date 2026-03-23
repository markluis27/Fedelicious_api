using System.Collections.Generic;
using System.Linq;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;

namespace Fedelicious_api.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IGenericRepository<reservations> _reservationRepo;
        private readonly IGenericRepository<customers> _customerRepo;

        public ReservationService(
            IGenericRepository<reservations> reservationRepo,
            IGenericRepository<customers> customerRepo)
        {
            _reservationRepo = reservationRepo;
            _customerRepo = customerRepo;
        }

        public IEnumerable<reservations> GetAllReservations()
        {
            return _reservationRepo.GetAll();
        }

        public reservations GetReservationById(int id)
        {
            return _reservationRepo.GetById(id);
        }

        public IEnumerable<reservations> GetReservationsByCustomer(int customerId)
        {
            return _reservationRepo.GetAll()
                .Where(r => r.customer_id == customerId);
        }

        public reservations CreateReservation(reservations newReservation)
        {
            if (newReservation == null) return null;

            if (string.IsNullOrWhiteSpace(newReservation.reservation_status))
                newReservation.reservation_status = "Pending";

            bool added = _reservationRepo.Add(newReservation);
            return added ? newReservation : null;
        }

        public bool UpdateReservation(reservations updatedReservation)
        {
            if (updatedReservation == null) return false;
            return _reservationRepo.Update(updatedReservation);
        }

        public bool DeleteReservation(int id)
        {
            return _reservationRepo.Delete(id);
        }

        public string GetCustomerEmail(int customerId)
        {
            var customer = _customerRepo.GetById(customerId);
            return customer?.email ?? "";
        }
    }
}