using Logging;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Notifications.API.BackgroundTasks;
using Notifications.API.Data;
using Notifications.API.MediatrNotifications.Answers;
using Notifications.API.MediatrNotifications.Questions;
using Notifications.API.MediatrNotifications.Users;
using Notifications.API.Options;
using RabbitMqEventBus.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("NotificationStorage");
    if (connectionString == null)
        throw new ArgumentNullException(nameof(connectionString));

    opt.UseNpgsql(connectionString);
});
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddRabbitConnection(builder.Configuration, "RabbitConnection");
builder.Services.AddMessageHandler(builder.Configuration, "RabbitHandler");
builder.Services.AddHandlerManager(opt =>
{
    opt.AddNotificationForMessageType<UserCreatedNotification>("UserCreated");

    opt.AddNotificationForMessageType<AnswerCreatedNotification>("AnswerCreated");

    opt.AddNotificationForMessageType<QuestionCreatedNotification>("QuestionCreated");
    opt.AddNotificationForMessageType<QuestionUpdatedNotification>("QuestionUpdated");
    opt.AddNotificationForMessageType<QuestionDeletedNotification>("QuestionDeleted");
});

builder.Services.AddHostedService<IntegrationMessageHandlerHostedService>();
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
builder.Services.AddSerilog(builder.Configuration, builder.Environment);

builder.WebHost.UseSerilog();

var app = builder.Build();
await MigrateDb(app);
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
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await context?.Database?.MigrateAsync();
}