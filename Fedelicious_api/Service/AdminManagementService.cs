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
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<dynamic>> GetAllAdminsAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Dynamic para hindi isama ang password sa response
                string sql = "SELECT admin_id, full_name, username, role FROM admins ORDER BY role DESC, full_name ASC";
                return await db.QueryAsync<dynamic>(sql);
            }
        }

        public async Task<string> CreateAdminAsync(admins newAdmin)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Check kung existing na ang username
                string checkSql = "SELECT COUNT(1) FROM admins WHERE username = @username";
                int exists = await db.ExecuteScalarAsync<int>(checkSql, new { username = newAdmin.username });

                if (exists > 0)
                {
                    return "Exists";
                }

                // Insert admin
                string insertSql = @"
                    INSERT INTO admins (full_name, username, password_hash, role) 
                    VALUES (@full_name, @username, @password_hash, @role)";

                var result = await db.ExecuteAsync(insertSql, newAdmin);
                return result > 0 ? "Success" : "Failed";
            }
        }

        public async Task<bool> DeleteAdminAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM admins WHERE admin_id = @Id";
                var result = await db.ExecuteAsync(sql, new { Id = id });
                return result > 0;
            }
        }
    }
}