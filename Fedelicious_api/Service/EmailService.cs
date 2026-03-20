using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace Fedelicious_api.Service
{
    public class EmailService : IEmailService
    {
        // Palitan ang _appPassword ng iyong totoong 16-digit app password
        private readonly string _senderEmail = "fedelicous.wings@gmail.com";
        private readonly string _appPassword = "synw qgcc lsrr fuez";

        // 1. GENERIC SEND METHOD (Single implementation lang para malinis)
        public void Send(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Fedelicious Wings", _senderEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

                using (var client = new SmtpClient())
                {
                    // Mahalaga: Port 587 at StartTls ang gamit sa Gmail
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_senderEmail, _appPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Lalabas ito sa Output window ng Visual Studio kung mag-fail ang SMTP
                System.Diagnostics.Debug.WriteLine("SMTP Error: " + ex.Message);
                throw; // I-throw ulit para malaman ng Controller kung bakit nag-fail
            }
        }

        // 2. REGISTRATION VERIFICATION
        public void SendVerificationEmail(string email, string token)
        {
            string verificationLink = $"https://localhost:7201/api/auth/verify?token={token}";
            string body = $@"<h2>Email Verification</h2>
                            <p>Welcome to Fedelicious! Click the link to verify:</p>
                            <a href='{verificationLink}'>Verify Account</a>";
            Send(email, "Fedelicious | Verify Your Email", body, true);
        }

        // 3. ORDER CONFIRMATION
        public void SendOrderConfirmation(string email, int orderId, decimal totalAmount)
        {
            string body = $@"<div style='font-family: Arial; border: 1px solid #ddd; padding: 20px;'>
                                <h2 style='color: #ea5b0c;'>Order Confirmed!</h2>
                                <p>Order ID: <strong>#{orderId}</strong></p>
                                <p>Total Amount: <strong>₱{totalAmount:N2}</strong></p>
                                <p>We are now processing your order. Thank you!</p>
                            </div>";
            Send(email, "Fedelicious | Order Confirmation", body, true);
        }

        // 4. ORDER STATUS UPDATE (ITO ANG TINATAWAG SA CONTROLLER)
        public void SendOrderStatusUpdate(string email, int orderId, string status, string statusMessage)
        {
            string body = $@"<div style='font-family: Arial; border: 1px solid #ddd; padding: 20px;'>
                                <h2 style='color: #ea5b0c;'>Order Update: {status}</h2>
                                <p>Hi! Your order <strong>#{orderId}</strong> status is now: <strong>{status}</strong>.</p>
                                <p>{statusMessage}</p>
                                <hr>
                                <p style='font-size: 12px; color: #888;'>Fedelicious Wings - Tanauan City</p>
                            </div>";

            // Siguraduhin na tinatawag ang Send method sa itaas
            Send(email, $"Fedelicious | Order #{orderId} Status Update", body, true);
        }
    }
}