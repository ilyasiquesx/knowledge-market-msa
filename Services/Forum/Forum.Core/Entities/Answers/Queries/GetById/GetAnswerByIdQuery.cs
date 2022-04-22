using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Answers.Queries.GetById;

public class GetAnswerByIdQuery : IRequest<AnswerDto>
{
    public long Id { get; }

    public GetAnswerByIdQuery(long id)
    {
        Id = id;
    }
}

public class GetAnswerByIdQueryHandler : IRequestHandler<GetAnswerByIdQuery, AnswerDto>
{
    private readonly IDomainContext _context;

    public GetAnswerByIdQueryHandler(IDomainContext context)
    {
        _context = context;
    }

    public async Task<AnswerDto> Handle(GetAnswerByIdQuery request, CancellationToken cancellationToken)
    {
        var answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        ThrowIf.Null(answer, $"There is no answer with such id: {request.Id}");

        return new AnswerDto
        {
            Content = answer!.Content,
            AuthorId = answer!.AuthorId
        };
    }
}