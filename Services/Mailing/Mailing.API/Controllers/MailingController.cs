using Mailing.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mailing.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MailingController : ControllerBase
{
    private readonly MailingContext _context;

    public MailingController(MailingContext context)
    {
        _context = context;
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id)
    {
        if (Guid.TryParse(id, out var result))
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                @$"UPDATE ""Users"" SET ""IsSubscribedForMailing""=FALSE WHERE ""Id""={id}");
        }

        return NoContent();
    }
}