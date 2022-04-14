using Forum.Core.Entities.Answers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers.Answers;

public class AnswersController : ApiController
{
    public AnswersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAnswerCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateAnswerCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await Mediator.Send(new DeleteAnswerCommand(id));
        return Ok();
    }
}