using System.Data;
using Dapper;
using Fedelicious_api.Model;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly string _connectionString;

        public PaymentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public bool SubmitGcashPayment(payments newPayment)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            if (newPayment == null)
                return false;

            if (string.IsNullOrWhiteSpace(newPayment.reference_number))
                return false;

            if (newPayment.amount <= 0)
                return false;

            if (newPayment.order_id == null && newPayment.reservation_id == null)
                return false;

            if (newPayment.paymentqr_id == null || newPayment.paymentqr_id <= 0)
                return false;

            int exists = db.ExecuteScalar<int>(
                "sp_Payments_CheckReference",
                new { reference_number = newPayment.reference_number },
                commandType: CommandType.StoredProcedure
            );

            if (exists > 0)
            {
                throw new Exception("REFERENCE_EXISTS");
            }

            int newId = db.ExecuteScalar<int>(
                "sp_Payments_Create",
                new
                {
                    order_id = newPayment.order_id,
                    reservation_id = newPayment.reservation_id,
                    payment_method = newPayment.payment_method,
                    amount = newPayment.amount,
                    reference_number = newPayment.reference_number,
                    customer_phone = newPayment.customer_phone,
                    payment_status = newPayment.payment_status,
                    payment_date = newPayment.payment_date,
                    paymentqr_id = newPayment.paymentqr_id
                },
                commandType: CommandType.StoredProcedure
            );

            if (newId <= 0)
            {
                throw new Exception("PAYMENT_INSERT_FAILED");
            }

            newPayment.payment_id = newId;
            return true;
        }

        public payments GetPaymentByOrderId(int orderId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            return db.QueryFirstOrDefault<payments>(
                "sp_Payments_GetByOrder",
                new { order_id = orderId },
                commandType: CommandType.StoredProcedure
            );
        }

        public payments GetPaymentByReservationId(int reservationId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            return db.QueryFirstOrDefault<payments>(
                "sp_Payments_GetByReservation",
                new { reservation_id = reservationId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}