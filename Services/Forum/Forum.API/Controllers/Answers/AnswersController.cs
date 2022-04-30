using Forum.Core.Entities.Answers.Commands.Create;
using Forum.Core.Entities.Answers.Commands.Delete;
using Forum.Core.Entities.Answers.Commands.Update;
using Forum.Core.Entities.Answers.Queries.GetById;
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
        var result = await Mediator.Send(command);
        var objectResult = result.Match<IActionResult>(
            _ => Ok(),
            validationResult => BadRequest(validationResult),
            notFoundResult => NotFound(notFoundResult));

        return objectResult;
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateAnswerCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        var objectResult = result.Match<IActionResult>(
            _ => NoContent(),
            validationResult => BadRequest(validationResult),
            notFoundResult => NotFound(notFoundResult),
            invalidState => BadRequest(invalidState));

        return objectResult;
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await Mediator.Send(new DeleteAnswerCommand(id));
        var objectResult = result.Match<IActionResult>(
            _ => NoContent(),
            notFoundResult => NotFound(notFoundResult),
            invalidState => BadRequest(invalidState));

        return objectResult;
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await Mediator.Send(new GetAnswerByIdQuery(id));
        var objectResult = result.Match<IActionResult>(
            dto => Ok(dto),
            notFoundResult => NotFound(notFoundResult));

        return objectResult;
    }
}