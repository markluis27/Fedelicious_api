using Fedelicious_api.Model;

namespace Fedelicious_api.Service
{
    public interface IPaymentService
    {
        bool SubmitGcashPayment(payments newPayment);
        payments GetPaymentByOrderId(int orderId);
        payments GetPaymentByReservationId(int resId);

    }
}