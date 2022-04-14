using System.Security.Claims;
using Mailing.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mailing.API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class MailingController : ControllerBase
{
    private readonly MailingContext _context;

    public MailingController(MailingContext context)
    {
        _context = context;
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateModel model)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new { Message = "User id must exist" });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound(new { Message = $"There is no user with such id: {userId}" });

        user.IsSubscribedForMailing = model.IsSubscribedForMailing;
        user.Email = model.Email;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}