using Authentication.API.Data;
using Authentication.API.Entities;
using Authentication.API.Token;
using Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMqEventBus.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuthContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("AuthStorage");
    Console.WriteLine(connectionString);
    if (connectionString == null)
        throw new ArgumentNullException(nameof(connectionString));

    opt.UseNpgsql(connectionString);
});

builder.Services.AddIdentity<User, IdentityRole>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequiredLength = 4;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<AuthContext>();

builder.Services.AddRabbitConnection(builder.Configuration, "RabbitConnection");
builder.Services.AddMessagePublisher(builder.Configuration, "RabbitPublisher");
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddSingleton<ITokenProvider, DefaultTokenProvider>();

builder.Services.AddSerilog(builder.Configuration, builder.Environment);
builder.WebHost.UseSerilog();

var app = builder.Build();
await MigrateDb(app);
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

async Task MigrateDb(IApplicationBuilder appBuilder)
{
    using var scope = appBuilder.ApplicationServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
    await context?.Database?.MigrateAsync();
}