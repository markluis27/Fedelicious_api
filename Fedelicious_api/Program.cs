using System.Data;
using System.Net;
using System.Net.Mail;
using Dapper;
using Fedelicious_api.Repository;
using Fedelicious_api.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FedeliciousPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAdminManagementService, AdminManagementService>();

SqlMapper.AddTypeHandler(new TimeSpanHandler());

var app = builder.Build();

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

public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value;
    }

    public override TimeSpan Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            return TimeSpan.Zero;

        return TimeSpan.Parse(value.ToString()!);
    }
}