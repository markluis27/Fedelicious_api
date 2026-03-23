using System.Data;
using Dapper;
using Fedelicious_api.Model;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Service
{
    public class AdminManagementService : IAdminManagementService
    {
        private readonly string _connectionString;

        public AdminManagementService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<dynamic>> GetAllAdminsAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            return await db.QueryAsync<dynamic>(
                "sp_admins_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<string> CreateAdminAsync(admins newAdmin)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            int exists = await db.ExecuteScalarAsync<int>(
                "sp_admins_CheckUsername",
                new { username = newAdmin.username },
                commandType: CommandType.StoredProcedure
            );

            if (exists > 0)
            {
                return "Exists";
            }

            int result = await db.ExecuteAsync(
                "sp_admins_Add",
                new
                {
                    full_name = newAdmin.full_name,
                    username = newAdmin.username,
                    password = newAdmin.password,
                    role = newAdmin.role
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0 ? "Success" : "Failed";
        }

        public async Task<bool> DeleteAdminAsync(int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            int result = await db.ExecuteAsync(
                "sp_admins_Delete",
                new { admin_id = id },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
    }
}