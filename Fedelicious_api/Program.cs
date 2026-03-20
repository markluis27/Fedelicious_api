using System.Data;
using System.Net;
using System.Net.Mail;
using Dapper;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using Fedelicious_api.Service;
using Microsoft.Data.SqlClient;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS - Pinapayagan ang HTML mo na kumausap sa API
builder.Services.AddCors(options => {
    options.AddPolicy("FedeliciousPolicy", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. CONTROLLERS + JSON FIX (Para hindi mag-undefined ang ID sa JS)
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        // Pinapanatili ang original property names (e.g., reservation_id)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. EMAIL SETTINGS
var emailSettings = builder.Configuration.GetSection("EmailSettings");
var email = emailSettings["Email"] ?? "";
var password = emailSettings["Password"] ?? "";
var host = emailSettings["Host"] ?? "smtp.gmail.com";
var portStr = emailSettings["Port"] ?? "587";
int.TryParse(portStr, out int port);

builder.Services.AddFluentEmail(email, "Fedelicious Wings")
    .AddSmtpSender(() => new SmtpClient(host)
    {
        Port = port,
        Credentials = new NetworkCredential(email, password),
        EnableSsl = true
    });

// 4. REPOSITORIES & SERVICES
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
// Add this line to register your new service!
builder.Services.AddScoped<IAdminManagementService, AdminManagementService>();

// 5. DAPPER TYPE HANDLER (Para sa TimeSpan/Time error)
SqlMapper.AddTypeHandler(new TimeSpanHandler());

var app = builder.Build();

// 6. MIDDLEWARE PIPELINE
// Laging unahin ang CORS bago ang Routing/Auth
app.UseCors("FedeliciousPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// 7. TIME SPAN HANDLER CLASS (Dapat nasa labas ng Program logic)
public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
        => parameter.Value = value;

    public override TimeSpan Parse(object value)
    {
        if (value == null || value == DBNull.Value) return TimeSpan.Zero;
        return TimeSpan.Parse(value.ToString());
    }
}