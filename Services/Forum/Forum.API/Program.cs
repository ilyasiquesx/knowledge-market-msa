using FluentValidation;
using FluentValidation.AspNetCore;
using Forum.API.BackgroundTasks;
using Forum.API.Middlewares;
using Forum.API.Options;
using Forum.Core.Entities.Users.Notifications;
using Forum.Core.Services;
using Forum.Infrastructure.Data;
using Forum.Infrastructure.Services;
using Logging;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMqEventBus.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ForumContext>(
    opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("ForumStorage")));
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var options = new JwtOptions();
        builder.Configuration.GetSection("JwtOptions").Bind(options);

        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = options.Issuer,
            ValidAudience = options.Audience,
            IssuerSigningKey = options.GetSecurityKey()
        };
    });

builder.Services.AddRabbitConnection(builder.Configuration, "RabbitConnection");
builder.Services.AddMessagePublisher(builder.Configuration, "RabbitPublisher");
builder.Services.AddMessageHandler(builder.Configuration, "RabbitHandler");
builder.Services.AddHandlerManager(opt =>
{
    opt.AddNotificationForMessageType<CreateUserNotification>("UserCreated");
    opt.AddNotificationForMessageType<SampleUserNotification>("SampleUser");
});
builder.Services.AddScoped<IDomainContext, ForumContext>();
builder.Services.AddTransient<IIntegrationEventService, IntegrationEventService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IDateService, DateService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<MessageHandlerHostedService>();
builder.Services.AddFluentValidation();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSerilog(builder.Configuration, builder.Environment);

builder.WebHost.UseSerilog();

var app = builder.Build();
await MigrateDb(app);
app.UseMiddleware<UnhandledExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

async Task MigrateDb(IApplicationBuilder appBuilder)
{
    using var scope = appBuilder.ApplicationServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ForumContext>();
    if (context.Database.IsRelational())
    {
        await context?.Database?.MigrateAsync();
    }
}

public partial class Program
{
}