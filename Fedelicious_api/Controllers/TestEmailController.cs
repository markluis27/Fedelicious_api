using Microsoft.AspNetCore.Mvc;
using FluentEmail.Core;
using System.Threading.Tasks;

namespace Fedelicious_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IFluentEmail _fluentEmail;

        public TestEmailController(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        [HttpGet("send")]
        public async Task<IActionResult> SendTestEmail()
        {
            var response = await _fluentEmail
                .To("fulinaramarkluis@gmail.com")
                .Subject("Fedelicious Test Email")
                .Body("<h2>Email Test Successful</h2><p>If you see this, SMTP is working.</p>", true)
                .SendAsync();

            if (response.Successful)
            {
                return Ok("Email sent successfully!");
            }
            else
            {
                return BadRequest(response.ErrorMessages);
            }
        }
    }
}