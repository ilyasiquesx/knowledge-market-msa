using Forum.Core.Entities.Questions.Commands;
using Forum.Core.Entities.Questions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers.Questions;

public class QuestionsController : ApiController
{
    public QuestionsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await Mediator.Send(new GetQuestionQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await Mediator.Send(new DeleteQuestionCommand(id));
        return Ok();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateQuestionCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);
        return NoContent();
    }
}