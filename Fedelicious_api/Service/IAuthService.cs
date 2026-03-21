using Fedelicious_api.Model;

namespace Fedelicious_api.Service
{
    public interface IAuthService
    {
        bool RegisterCustomer(customers newCustomer);
        customers LoginCustomer(string email, string password);
        admins LoginAdmin(string username, string password);
        Task<bool> ForgotPassword(string email);
        bool ResetPassword(string token, string password);


    }
}
