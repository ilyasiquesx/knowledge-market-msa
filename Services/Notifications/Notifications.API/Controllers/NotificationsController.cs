using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notifications.API.Data;
using Notifications.API.Dto;

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
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .ToListAsync();

        var dto = unreadNotificationsForUser
            .Select(n => new
            {
                IsRead = n.IsRead,
                Content = JsonConvert.DeserializeObject<NotificationDto>(n.Content)
            });

        return Ok(dto);
    }

    private string GetUserId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}