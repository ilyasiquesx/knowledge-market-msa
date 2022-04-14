using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Configuration.AddJsonFile(builder.Configuration.GetValue<bool>("IsInDocker")
    ? "ocelot.docker.json"
    : "ocelot.json");

builder.Services.AddOcelot();

var app = builder.Build();

app.UseCors(opt => opt
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseOcelot()
    .Wait();

app.Run();