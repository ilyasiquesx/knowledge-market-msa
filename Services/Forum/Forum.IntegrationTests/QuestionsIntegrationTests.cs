using System.Net.Http.Json;
using System.Threading.Tasks;
using Forum.Core.Entities.Questions.Commands.Create;
using Forum.Core.Entities.Questions.Queries.Get;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Forum.IntegrationTests;

public class QuestionsIntegrationTests
{
    private readonly ForumWebApplicationFactory _factory = new();

    [Fact]
    public async Task GetQuestionById_ShouldReturnQuestion_IfQuestionExists()
    {
        var client = _factory.CreateClient();
        var testQuestion = _factory.SeedAndGetTestQuestion();

        var response = await client.GetAsync($"/questions/{testQuestion!.Id}");

        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<QuestionDto>();
        Assert.Equal(testQuestion.Id, dto!.Id);
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnStatusCodeOk_IfRequestIsValid()
    {
        var client = _factory.CreateClient();
        var user = _factory.SeedAndGetTestUser();
        var token = _factory.GetToken(user!);

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var createQuestionCommand = new CreateQuestionCommand()
        {
            Title = "Test Title",
            Content = "Test content"
        };

        var response = await client.PostAsJsonAsync("/questions", createQuestionCommand);
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        Assert.False(true);
    }
}