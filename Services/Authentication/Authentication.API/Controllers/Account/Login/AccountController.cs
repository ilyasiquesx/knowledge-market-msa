using Authentication.API.Entities;
using Authentication.API.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers.Account.Login;

public class AccountController : ApiController
{
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<User> _manager;

    public AccountController(ITokenProvider tokenProvider, UserManager<User> manager)
    {
        _tokenProvider = tokenProvider;
        _manager = manager;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            return BadRequest(new { Message = "The credentials for user are invalid" });

        var user = await _manager.FindByNameAsync(model.Username);
        if (user == null)
            return NotFound(new { Message = "There is no user with such username" });

        var loginResult = await _manager.CheckPasswordAsync(user, model.Password);
        if (!loginResult)
            return BadRequest(new { Message = "The credentials for user are wrong" });

        var token = _tokenProvider.GetTokenForUser(user);
        return Ok(new
        {
            AccessToken = token,
            Username = user.UserName,
            Id = user.Id,
        });
    }
}