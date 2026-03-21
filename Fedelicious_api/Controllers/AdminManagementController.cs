using Fedelicious_api.Model;
using Fedelicious_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminManagementController : ControllerBase
    {
        private readonly IAdminManagementService _adminService;

        public AdminManagementController(IAdminManagementService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var admins = await _adminService.GetAllAdminsAsync();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting admins.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] admins newAdmin)
        {
            try
            {
                if (newAdmin == null)
                    return BadRequest(new { message = "Invalid admin data." });

                if (string.IsNullOrWhiteSpace(newAdmin.full_name) ||
                    string.IsNullOrWhiteSpace(newAdmin.username) ||
                    string.IsNullOrWhiteSpace(newAdmin.password))
                {
                    return BadRequest(new { message = "Full name, username, and password are required." });
                }

                if (string.IsNullOrWhiteSpace(newAdmin.role))
                {
                    newAdmin.role = "Admin";
                }

                string result = await _adminService.CreateAdminAsync(newAdmin);

                if (result == "Exists")
                    return BadRequest(new { message = "Username already exists." });

                if (result == "Success")
                    return Ok(new { message = "Admin added successfully." });

                return BadRequest(new { message = "Failed to add admin." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating admin.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                if (id == 1)
                    return BadRequest(new { message = "You cannot delete the main admin." });

                bool deleted = await _adminService.DeleteAdminAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Admin not found." });

                return Ok(new { message = "Admin deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting admin.", error = ex.Message });
            }
        }
    }
}