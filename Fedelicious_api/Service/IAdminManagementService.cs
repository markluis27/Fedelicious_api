using Fedelicious_api.Model;

namespace Fedelicious_api.Service
{
    public interface IAdminManagementService
    {
        Task<IEnumerable<dynamic>> GetAllAdminsAsync();
        Task<string> CreateAdminAsync(admins newAdmin);
        Task<bool> DeleteAdminAsync(int id);
    }
}