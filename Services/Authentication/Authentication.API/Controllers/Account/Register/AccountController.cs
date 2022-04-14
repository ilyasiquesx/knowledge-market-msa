using Authentication.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RabbitMqEventBus.MessagePublisher;

namespace Authentication.API.Controllers.Account.Register;

public class AccountController : ApiController
{
    private readonly UserManager<User> _userManager;
    private readonly IMessagePublisher _messagePublisher;

    public AccountController(UserManager<User> userManager,
        IMessagePublisher messagePublisher)
    {
        _userManager = userManager;
        _messagePublisher = messagePublisher;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!string.Equals(model.Password, model.ConfirmPassword))
            return BadRequest(new { Message = "Passwords must match" });

        var user = new User
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(new { Message = "Something went wrong" });

        await _messagePublisher.PublishAsync("UserCreated", new
        {
            UserId = user.Id,
            Username = user.UserName,
            Date = DateTime.UtcNow,
            model.IsSubscribedForMailing,
            user.Email
        });

        return Ok();
    }
}