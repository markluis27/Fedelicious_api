using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using System.Linq;
using System.Collections.Generic;

namespace Fedelicious_api.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<payments> _paymentRepo;

        public PaymentService(IGenericRepository<payments> paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public bool SubmitGcashPayment(payments newPayment)
        {
            if (string.IsNullOrEmpty(newPayment.reference_number)) return false;

            var existing = _paymentRepo.GetAll().Any(p => p.reference_number == newPayment.reference_number);
            if (existing) return false;

            return _paymentRepo.Add(newPayment);
        }

        public payments GetPaymentByOrderId(int orderId)
        {
            return _paymentRepo.GetAll().FirstOrDefault(p => p.order_id == orderId);
        }

        public payments GetPaymentByReservationId(int resId)
        {
            return _paymentRepo.GetAll().FirstOrDefault(p => p.reservation_id == resId);
        }
    }
}