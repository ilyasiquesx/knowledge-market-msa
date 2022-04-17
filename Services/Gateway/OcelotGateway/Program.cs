using Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Configuration.AddJsonFile(builder.Configuration.GetValue<bool>("IsInDocker")
    ? "ocelot.docker.json"
    : "ocelot.json");

builder.Services.AddOcelot();

builder.Services.AddSerilog(builder.Configuration, builder.Environment);
builder.WebHost.UseSerilog();

var app = builder.Build();

app.UseCors(opt => opt
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseOcelot()
    .Wait();

app.Run();