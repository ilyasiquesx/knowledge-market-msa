using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Answers.Queries.GetById;

public class GetAnswerByIdQuery : IRequest<OneOf<AnswerDto, NotFoundResult>>
{
    public long Id { get; }

    public GetAnswerByIdQuery(long id)
    {
        Id = id;
    }
}

public class GetAnswerByIdQueryHandler : IRequestHandler<GetAnswerByIdQuery, OneOf<AnswerDto, NotFoundResult>>
{
    private readonly IDomainContext _context;

    public GetAnswerByIdQueryHandler(IDomainContext context)
    {
        _context = context;
    }

    public async Task<OneOf<AnswerDto, NotFoundResult>> Handle(GetAnswerByIdQuery request, CancellationToken cancellationToken)
    {
        var answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (answer == null)
            return new NotFoundResult($"There is no answer with such id: {request.Id}");

        return new AnswerDto
        {
            Content = answer.Content,
            AuthorId = answer.AuthorId
        };
    }
}