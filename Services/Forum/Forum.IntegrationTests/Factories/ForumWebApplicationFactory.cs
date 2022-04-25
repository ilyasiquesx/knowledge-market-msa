using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Forum.API.Options;
using Forum.Core.Entities.Questions;
using Forum.Core.Entities.Users;
using Forum.Infrastructure.Data;
using Forum.IntegrationTests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RabbitMqEventBus.MessageHandler;
using RabbitMqEventBus.MessagePublisher;

namespace Forum.IntegrationTests.Factories;

public class ForumWebApplicationFactory : WebApplicationFactory<Program>
{
    private JwtOptions JwtOptions { get; } = new();
    private IServiceProvider _serviceProvider = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(cs =>
        {
            var typesArray = new Type[]
            {
                typeof(DbContextOptions<ForumContext>),
            };

            var descriptors = cs.Where(d => typesArray.Contains(d.ServiceType)).ToList();

            foreach (var descriptor in descriptors)
            {
                cs.Remove(descriptor);
            }

            cs.AddDbContext<ForumContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            cs.AddTransient<IMessagePublisher, MockHandlerPublisher>();
            cs.AddTransient<IMessageHandler, MockHandlerPublisher>();

            var sp = cs.BuildServiceProvider();
            _serviceProvider = sp;

            using var scope = sp.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            SetJwtOptions(config);
        });

        builder.ConfigureServices(cs => { });
    }

    public string GetToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        var utcNow = DateTime.UtcNow;
        var expires = utcNow.AddMinutes(60);
        var credentials = new SigningCredentials(JwtOptions.GetSecurityKey(), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: JwtOptions.Issuer,
            audience: JwtOptions.Audience,
            notBefore: utcNow,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return encodedJwt;
    }

    public User? SeedAndGetTestUser()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ForumContext>();
        if (context.Users.Any())
            return context.Users.FirstOrDefault();

        var user = new User { Id = "user-id", Username = "username" };
        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    public Question? SeedAndGetTestQuestion()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ForumContext>();
        if (context.Questions.Any())
            return context.Questions.FirstOrDefault();

        var question = new Question
        {
            Title = "Test question",
            Content = "Test content",
            Author = new User
            {
                Id = "author-id",
                Username = "TestAuthor"
            }
        };

        context.Questions.Add(question);
        context.SaveChanges();
        return question;
    }

    private void SetJwtOptions(IConfiguration configuration)
    {
        configuration.GetSection("JwtOptions").Bind(JwtOptions);
    }
}