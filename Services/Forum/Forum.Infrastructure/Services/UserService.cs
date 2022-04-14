using System.Security.Claims;
using Forum.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Forum.Infrastructure.Services;

public class UserService : IUserService
{
    public string? UserId { get; }

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value.ToString();
    }
}