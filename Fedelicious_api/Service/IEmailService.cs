namespace Fedelicious_api.Service
{
    public interface IEmailService
    {
        // Generic Send method (Gagamitin ng iba pang methods sa loob)
        void Send(string to, string subject, string body, bool isHtml = true);

        // Registration Verification
        void SendVerificationEmail(string email, string token);

        // Specific Order Methods
        void SendOrderConfirmation(string email, int orderId, decimal totalAmount);
        void SendOrderStatusUpdate(string email, int orderId, string status, string statusMessage);
    }
}