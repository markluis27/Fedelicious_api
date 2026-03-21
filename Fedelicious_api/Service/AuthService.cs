using System;
using System.Linq;
using System.Threading.Tasks;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using FluentEmail.Core;
using System.Diagnostics;

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

        // ================= REGISTER =================
        public bool RegisterCustomer(customers newCustomer)
        {
            try
            {
                var existing = _customerRepo.GetAll()
                    .FirstOrDefault(c => c.email == newCustomer.email);

                if (existing != null) return false;

                newCustomer.is_verified = false;

                bool isSaved = _customerRepo.Add(newCustomer);
                if (!isSaved) return false;

                var savedCustomer = _customerRepo.GetAll()
                    .OrderByDescending(c => c.customer_id)
                    .FirstOrDefault(c => c.email == newCustomer.email);

                if (savedCustomer == null) return false;

                string token = Guid.NewGuid().ToString();

                _tokenRepo.Add(new verification_tokens
                {
                    token = token,
                    customer_id = savedCustomer.customer_id,
                    expiry_date = DateTime.Now.AddHours(24)
                });

                _ = SendVerificationEmail(savedCustomer.email, token);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[REGISTER ERROR]: {ex.Message}");
                return false;
            }
        }

        private async Task SendVerificationEmail(string email, string token)
        {
            try
            {
                string verifyLink = $"https://localhost:7201/api/Auth/verify?token={token}";

                await _fluentEmail
                    .To(email)
                    .Subject("Fedelicious | Verify Your Account")
                    .Body($@"
                        <h2>Verify Your Account</h2>
                        <p>Click below:</p>
                        <a href='{verifyLink}'>VERIFY</a>
                    ", true)
                    .SendAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EMAIL ERROR]: {ex.Message}");
            }
        }

        // ================= LOGIN =================
        public customers LoginCustomer(string email, string password)
        {
            return _customerRepo.GetAll()
                .FirstOrDefault(c =>
                    c.email == email &&
                    c.password == password &&
                    c.is_verified == true);
        }

        public admins LoginAdmin(string username, string password)
        {
            return _adminRepo.GetAll()
                .FirstOrDefault(a =>
                    a.username == username &&
                    a.password == password);
        }

        // ================= FORGOT PASSWORD =================
        public async Task<bool> ForgotPassword(string email)
        {
            try
            {
                var user = _customerRepo.GetAll()
                    .FirstOrDefault(c => c.email == email);

                if (user == null) return false;

                string token = Guid.NewGuid().ToString();

                _tokenRepo.Add(new verification_tokens
                {
                    token = token,
                    customer_id = user.customer_id,
                    expiry_date = DateTime.Now.AddMinutes(15)
                });

                // ✅ FIXED LINK (THIS IS THE MAIN FIX)
                string resetLink = $"http://127.0.0.1:5500/reset-password.html?token={token}&email={email}";

                await _fluentEmail
                    .To(email)
                    .Subject("Fedelicious | Reset Password")
                    .Body($@"
<div style='background:#053b2d;padding:30px;font-family:Poppins,Arial;'>

    <div style='max-width:500px;margin:auto;background:#0d4d3c;padding:30px;border-radius:10px;text-align:center;color:white;'>

        <h1>Fedelicious</h1>
        <h2>Reset Your Password</h2>

        <p>Click the button below to reset your password.</p>

        <a href='{resetLink}' 
           style='display:inline-block;
                  padding:14px 25px;
                  background:#ff7a00;
                  color:white;
                  text-decoration:none;
                  border-radius:6px;
                  font-weight:bold;'>
            RESET PASSWORD
        </a>

        <p style='margin-top:20px;font-size:12px;'>
            This link expires in 15 minutes.
        </p>

    </div>

</div>
", true)
                    .SendAsync();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FORGOT PASSWORD ERROR]: {ex.Message}");
                return false;
            }
        }

        // ================= RESET PASSWORD =================
        public bool ResetPassword(string token, string password)
        {
            var tokenEntry = _tokenRepo.GetAll()
                .FirstOrDefault(t => t.token == token);

            if (tokenEntry == null || DateTime.Now > tokenEntry.expiry_date)
                return false;

            var user = _customerRepo.GetById(tokenEntry.customer_id);
            if (user == null) return false;

            user.password = password;
            _customerRepo.Update(user);

            // expire token
            tokenEntry.expiry_date = DateTime.Now.AddSeconds(-1);
            _tokenRepo.Update(tokenEntry);

            return true;
        }
    }
}