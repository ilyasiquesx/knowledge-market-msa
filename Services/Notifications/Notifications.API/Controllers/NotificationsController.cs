using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;

namespace Notifications.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly ApplicationContext _context;

    public NotificationsController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetForUser()
    {
        var userId = GetUserId();
        var unreadNotificationsForUser = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(20)
            .ToListAsync();

        var dto = unreadNotificationsForUser
            .Select(n => new
            {
                n.IsRead,
                n.Content,
                CreatedAt = n.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm")
            });

        return Ok(dto);
    }

    [HttpPut]
    public async Task<IActionResult> SetAllReadForUser()
    {
        var userId = GetUserId();
        await _context.Database.ExecuteSqlRawAsync(
            @$"UPDATE ""Notifications"" SET ""IsRead""=TRUE WHERE ""UserId""='{userId}'");
        return NoContent();
    }

    private string GetUserId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}