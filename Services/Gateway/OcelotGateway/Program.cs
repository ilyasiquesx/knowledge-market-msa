using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(builder.Configuration.GetValue<bool>("IsInDocker")
    ? "ocelot.docker.json"
    : "ocelot.json");

builder.Services.AddOcelot();

var app = builder.Build();
app.UseOcelot()
    .Wait();

app.Run();