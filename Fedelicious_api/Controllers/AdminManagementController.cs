using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Fedelicious_api.Model;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminManagementController : ControllerBase
    {
        private readonly string _connectionString = "Server=LAPTOP-OU71PFMJ\\SQLEXPRESS;Database=Fedelicious;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

        // 1. GET ALL ADMINS
        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Tinawag ang SP. Note: Kung gusto mo ng specific ORDER BY tulad ng dati, 
                // ilagay mo ang ORDER BY clause sa loob mismo ng sp_admins_GetAll mo sa SQL.
                var adminsList = await db.QueryAsync<dynamic>(
                    "sp_admins_GetAll",
                    commandType: CommandType.StoredProcedure
                );
                return Ok(adminsList);
            }
        }

        // 2. CREATE NEW ADMIN
        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] admins newAdmin)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Naiwan ko ang raw SQL na ito kasi wala ka pang SP na pang-check ng existing username.
                // Pwede ka ring gumawa ng SP para dito sa future!
                string checkSql = "SELECT COUNT(1) FROM admins WHERE username = @username";
                int exists = await db.ExecuteScalarAsync<int>(checkSql, new { username = newAdmin.username });

                if (exists > 0)
                {
                    return BadRequest(new { message = "Username is already taken." });
                }

                // Dito natin ipapasa yung parameters para sa SP
                var parameters = new
                {
                    full_name = newAdmin.full_name,
                    username = newAdmin.username,
                    password = newAdmin.password
                    // role = newAdmin.role  <-- I-uncomment mo ito KUNG idinagdag mo na ang @role sa sp_admins_Add mo
                };

                var result = await db.ExecuteAsync(
                    "sp_admins_Add",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result > 0)
                {
                    return Ok(new { message = "Admin successfully created." });
                }

                return StatusCode(500, new { message = "Failed to create admin in the database." });
            }
        }

        // 3. DELETE ADMIN
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                if (id == 1) // Master admin protection
                {
                    return BadRequest(new { message = "You cannot delete the master Super Admin account." });
                }

                // Importante: Ang parameter name dito dapat mag-match sa variable name sa SP mo (@admin_id)
                var result = await db.ExecuteAsync(
                    "sp_admins_Delete",
                    new { admin_id = id },
                    commandType: CommandType.StoredProcedure
                );

                if (result > 0)
                {
                    return Ok(new { message = "Admin deleted successfully." });
                }

                return NotFound(new { message = "Admin not found." });
            }
        }
    }
}