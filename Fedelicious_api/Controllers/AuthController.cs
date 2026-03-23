using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using Fedelicious_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGenericRepository<verification_tokens> _tokenRepo;
        private readonly IGenericRepository<customers> _customerRepo;

        public AuthController(
            IAuthService authService,
            IGenericRepository<verification_tokens> tokenRepo,
            IGenericRepository<customers> customerRepo)
        {
            _authService = authService;
            _tokenRepo = tokenRepo;
            _customerRepo = customerRepo;
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] customers newCustomer)
        {
            if (newCustomer == null)
                return BadRequest(new { message = "Invalid customer data." });

            bool registered = _authService.RegisterCustomer(newCustomer);

            if (!registered)
                return BadRequest(new { message = "Registration failed. Email may already exist." });

            return Ok(new { message = "Registration successful! Check your email for verification." });
        }

        [HttpPost("login/customer")]
        public IActionResult CustomerLogin([FromBody] LoginRequest request)
        {
            var customer = _authService.LoginCustomer(request.Email, request.Password);

            if (customer == null)
                return Unauthorized(new { message = "Invalid email/password or account not yet verified." });

            return Ok(new
            {
                message = "Customer login successful!",
                user = customer
            });
        }

        [HttpPost("login/admin")]
        public IActionResult AdminLogin([FromBody] LoginRequest request)
        {
            var admin = _authService.LoginAdmin(request.Email, request.Password);

            if (admin == null)
                return Unauthorized(new { message = "Invalid admin credentials." });

            return Ok(new
            {
                message = "Admin login successful!",
                user = admin
            });
        }

        [HttpGet("verify")]
        public IActionResult VerifyEmail([FromQuery] string token)
        {
            string frontendUrl = "http://127.0.0.1:5500";

            var tokenEntry = _tokenRepo.GetAll().FirstOrDefault(t => t.token == token);

            if (tokenEntry == null || DateTime.Now > tokenEntry.expiry_date)
                return Redirect($"{frontendUrl}/login.html?status=invalid_or_expired");

            var customer = _customerRepo.GetById(tokenEntry.customer_id);

            if (customer == null)
                return Redirect($"{frontendUrl}/login.html?status=user_not_found");

            customer.is_verified = true;
            _customerRepo.Update(customer);

            return Redirect($"{frontendUrl}/login.html?status=verified");
        }

        [HttpGet("getCustomerId/{fullname}")]
        public IActionResult GetCustomerId(string fullname)
        {
            var customer = _customerRepo.GetAll()
                .FirstOrDefault(c => c.full_name == fullname);

            if (customer == null)
                return NotFound(new { message = "Customer not found." });

            return Ok(new
            {
                customer_id = customer.customer_id,
                full_name = customer.full_name
            });
        }

        [HttpGet("getFullName/{id}")]
        public IActionResult GetFullName(int id)
        {
            var customer = _customerRepo.GetById(id);

            if (customer == null)
                return NotFound(new { message = "Customer ID not found." });

            return Ok(new
            {
                customer_id = customer.customer_id,
                full_name = customer.full_name
            });
        }
    }
}