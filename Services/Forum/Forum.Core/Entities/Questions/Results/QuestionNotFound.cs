namespace Forum.Core.Entities.Questions.Results;

public struct QuestionNotFound
{
    public string Message => $"There is no question with such id: {_id}";

    private readonly long _id;

    public QuestionNotFound(long id)
    {
        _id = id;
    }
}