using System;
using System.Linq;
using System.Threading.Tasks;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using FluentEmail.Core;
using System.Diagnostics; // For Debug.WriteLine

namespace Fedelicious_api.Service
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<customers> _customerRepo;
        private readonly IGenericRepository<admins> _adminRepo;
        private readonly IGenericRepository<verification_tokens> _tokenRepo;
        private readonly IFluentEmail _fluentEmail;

        public AuthService(
            IGenericRepository<customers> customerRepo,
            IGenericRepository<admins> adminRepo,
            IGenericRepository<verification_tokens> tokenRepo,
            IFluentEmail fluentEmail)
        {
            _customerRepo = customerRepo;
            _adminRepo = adminRepo;
            _tokenRepo = tokenRepo;
            _fluentEmail = fluentEmail;
        }

        public bool RegisterCustomer(customers newCustomer)
        {
            var existing = _customerRepo.GetAll().FirstOrDefault(c => c.email == newCustomer.email);
            if (existing != null) return false;

            newCustomer.is_verified = false;
            bool isSaved = _customerRepo.Add(newCustomer);

            if (isSaved)
            {
                var savedCustomer = _customerRepo.GetAll().FirstOrDefault(c => c.email == newCustomer.email);

                if (savedCustomer != null)
                {
                    string myToken = Guid.NewGuid().ToString();
                    var tokenEntry = new verification_tokens
                    {
                        token = myToken,
                        customer_id = savedCustomer.customer_id,
                        expiry_date = DateTime.Now.AddHours(24)
                    };

                    _tokenRepo.Add(tokenEntry);

                    // Send verification email
                    try
                    {
                        SendVerificationEmail(savedCustomer.email, myToken).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[SMTP ERROR]: {ex.Message}");
                    }
                }
            }
            return isSaved;
        }

        private async Task SendVerificationEmail(string email, string token)
        {
            try
            {
                string verifyLink = $"https://localhost:7201/api/Auth/verify?token={token}";

                var result = await _fluentEmail
                    .To(email)
            .Subject("Fedelicious | Verify Your Account")
            .Body($@"
                <div style='background-color: #f9f9f9; padding: 40px; text-align: center;'>
                    <div style='background-color: #ffffff; padding: 20px; border-radius: 10px; border: 1px solid #ddd; display: inline-block;'>
                        <h2 style='color: #09261c;'>Verify Your Account</h2>
                        <p>Click the button below to activate your account.</p>
                        <br>
                        <a href='{verifyLink}' 
                           target='_blank' 
                           style='display: inline-block; background-color: #ea5b0c; color: #ffffff; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                           VERIFY EMAIL NOW
                        </a>
                        <br><br>
                        <p style='font-size: 11px; color: #999;'>If the button is not clickable, please mark this email as 'Not Spam'.</p>
                    </div>
                </div>", isHtml: true)
            .SendAsync();

                if (!result.Successful)
                {
                    foreach (var error in result.ErrorMessages)
                    {
                        Debug.WriteLine($"[FLUENT ERROR]: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SMTP EXCEPTION]: {ex.Message}");
                throw;
            }
        }

        public customers LoginCustomer(string email, string password)
        {
            return _customerRepo.GetAll()
                .FirstOrDefault(c => c.email == email && c.password == password && c.is_verified == true);
        }

        public admins LoginAdmin(string username, string password)
        {
            return _adminRepo.GetAll()
                .FirstOrDefault(a => a.username == username && a.password == password);
        }
    }
}