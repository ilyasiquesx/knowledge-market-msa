using Forum.Core.Entities.Answers;
using Forum.Core.Entities.Questions;
using Forum.Core.Entities.Questions.Queries;
using Forum.Core.Entities.Questions.Queries.Get;
using Forum.Core.Entities.Questions.Queries.GetPaginated;
using Forum.Core.Entities.Users;

namespace Forum.Core.Entities;

public static class Builders
{
    public static class Answers
    {
        public static IEnumerable<AnswerDto> BuildAnswersDto(IEnumerable<Answer> answers, string requestedBy)
        {
            if (answers == null)
                yield break;

            foreach (var answer in answers)
            {
                yield return BuildAnswerDto(answer, requestedBy);
            }
        }

        public static AnswerDto BuildAnswerDto(Answer answer, string requestedBy)
        {
            var doesAuthorPresent = !string.IsNullOrEmpty(answer.AuthorId) && answer.Author != null;
            ThrowIf.False(doesAuthorPresent, "Answer can't has no author");

            return new AnswerDto
            {
                Id = answer.Id,
                Author = Users.BuildAuthorDto(answer.Author),
                Content = answer.Content,
                CreatedAt = answer.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
                UpdatedAt = answer.UpdatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
                AvailableToEdit = requestedBy == answer.AuthorId
            };
        }
    }

    public static class Users
    {
        public static AuthorDto BuildAuthorDto(User user)
        {
            return new AuthorDto
            {
                Username = user.Username,
                Id = user.Id
            };
        }
    }

    public static class Questions
    {
        public static QuestionDtoTiny BuildQuestionDto(Question question)
        {
            return new QuestionDtoTiny
            {
                Id = question.Id,
                Title = question.Title,
                Author = Users.BuildAuthorDto(question.Author),
                CreatedAt = question.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
                UpdatedAt = question.UpdatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
                AnswersCount = question.Answers.Count
            };
        }
    }
}