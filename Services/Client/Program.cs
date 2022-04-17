using System.Text;
using Client;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RabbitMqEventBus.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddRabbitConnection(opt =>
// {
//     opt.Username = "guest";
//     opt.Hostname = "localhost";
//     opt.Password = "guest";
//     opt.Port = 5672;
// });
//
// builder.Services.AddMessagePublisher(opt => { opt.ExchangeName = "actionConfig"; });
//
// builder.Services.AddMessageHandler(opt =>
// {
//     opt.ExchangeName = "actionConfig";
//     opt.QueueName = "MyQueue";
// });
//
// builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
// builder.Services.AddHandlerManager(opt =>
// {
//     opt.AddNotificationForMessageType<UserCreatedNotification>("UserCreated");
// });
//
// builder.Services.AddHostedService<BackgroundTask>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "KnowledgeMarkerAuthenticationServer",
        ValidAudience = "KnowledgeMarkerUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("NBE81A37CG3YYR67"))
    };
    opt.Events = new JwtBearerEvents
    {
        OnTokenValidated = context => { return Task.CompletedTask; },
        OnAuthenticationFailed = (context => { return Task.CompletedTask; })
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();