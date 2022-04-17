using System.Text;
using Logging;
using Mailing.API.BackgroundTasks;
using Mailing.API.Data;
using Mailing.API.MessageBuilding;
using Mailing.API.Notifications.Answers;
using Mailing.API.Notifications.Questions;
using Mailing.API.Notifications.Users;
using Mailing.API.Options;
using Mailing.API.Smtp;
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
builder.Services.AddSingleton<ISmtpSender, DefaultSmtpSender>();
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<MailingContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("MailingStorage");
    if (connectionString == null)
        throw new ArgumentNullException(nameof(connectionString));

    opt.UseNpgsql(connectionString);
});

builder.Services.AddHandlerManager(opt =>
{
    opt.AddNotificationForMessageType<QuestionCreatedNotification>("QuestionCreated");
    opt.AddNotificationForMessageType<QuestionUpdatedNotification>("QuestionUpdated");
    opt.AddNotificationForMessageType<QuestionDeletedNotification>("QuestionDeleted");

    opt.AddNotificationForMessageType<UserCreatedNotification>("UserCreated");
    opt.AddNotificationForMessageType<AnswerCreatedNotification>("AnswerCreated");
});

builder.Services.AddRabbitConnection(builder.Configuration, "RabbitConnection");
builder.Services.AddMessageHandler(builder.Configuration, "RabbitHandler");
builder.Services.AddHostedService<InboxMessageHandlerHostedService>();
builder.Services.AddHostedService<IntegrationMessageHandlerHostedService>();
builder.Services.AddSingleton<IMessageBuilderProvider, DefaultMessageBuilderProvider>();
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpOptions"));
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
    var context = scope.ServiceProvider.GetRequiredService<MailingContext>();
    await context?.Database?.MigrateAsync();
}