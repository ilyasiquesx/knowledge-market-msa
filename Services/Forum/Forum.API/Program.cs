using Forum.API.BackgroundTasks;
using Forum.API.Options;
using Forum.Core.Entities.Users.Notifications;
using Forum.Core.Services;
using Forum.Infrastructure.Data;
using Forum.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMqEventBus.DependencyInjection;

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
});
builder.Services.AddScoped<IDomainContext, ForumContext>();
builder.Services.AddTransient<IIntegrationEventService, IntegrationEventService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IDateService, DateService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<MessageHandlerHostedService>();

var app = builder.Build();
await MigrateDb(app);
app.Use(async (context, next) =>
{
    var logger = context.Request.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
    try
    {
        await next();
    }
    catch (Exception e)
    {
        logger.LogWarning(e, "Global exception handling");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            e.Message
        });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

async Task MigrateDb(IApplicationBuilder appBuilder)
{
    using var scope = appBuilder.ApplicationServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ForumContext>();
    await context?.Database?.MigrateAsync();
}