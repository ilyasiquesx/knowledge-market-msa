using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ApiController : ControllerBase
{
    protected readonly IMediator Mediator;

    public ApiController(IMediator mediator)
    {
        Mediator = mediator;
    }
}