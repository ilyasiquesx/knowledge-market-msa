using Forum.Core.Entities.Questions.Commands.Create;
using Forum.Core.Entities.Questions.Commands.Delete;
using Forum.Core.Entities.Questions.Commands.Update;
using Forum.Core.Entities.Questions.Queries.GetById;
using Forum.Core.Entities.Questions.Queries.GetPaginated;
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
        var result = await Mediator.Send(new GetQuestionByIdQuery(id));
        var objectResult = result.Match<IActionResult>(
            dto => Ok(dto),
            notFoundResult => NotFound(notFoundResult),
            invalidBehavior => BadRequest(invalidBehavior));
        
        return objectResult;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetWithPagination([FromQuery] Pagination pagination)
    {
        var result = await Mediator.Send(new GetQuestionsQuery(pagination));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionCommand command)
    {
        var result = await Mediator.Send(command);
        var objectResult = result.Match<IActionResult>(
            _ => Ok(),
            validationResult => BadRequest(validationResult),
            notFoundResult => NotFound(notFoundResult));
        
        return objectResult;
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateQuestionCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        var objectResult = result.Match<IActionResult>(
            _ => NoContent(),
            validationResult => BadRequest(validationResult),
            notFoundResult => NotFound(notFoundResult),
            invalidBehavior => BadRequest(invalidBehavior));

        return objectResult;
    }
    
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await Mediator.Send(new DeleteQuestionCommand(id));
        var objectResult = result.Match<IActionResult>(
            _ => NoContent(), 
            notFoundResult => NotFound(notFoundResult),
            invalidBehavior => BadRequest(invalidBehavior));
        
        return objectResult;
    }
}