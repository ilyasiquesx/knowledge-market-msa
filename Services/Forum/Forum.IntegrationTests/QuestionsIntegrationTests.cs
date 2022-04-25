using System.Net.Http.Json;
using System.Threading.Tasks;
using Forum.Core.Entities.Questions.Commands.Create;
using Forum.Core.Entities.Questions.Queries.Get;
using Forum.IntegrationTests.Factories;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Forum.IntegrationTests;

public class QuestionsIntegrationTests
{
    private readonly ForumWebApplicationFactory _factory = new();

    [Fact]
    public async Task GetQuestionById_ShouldReturnQuestion_IfQuestionExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var testQuestion = _factory.SeedAndGetTestQuestion();

        // Act
        var response = await client.GetAsync($"/questions/{testQuestion!.Id}");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<QuestionDto>();
        Assert.Equal(testQuestion.Id, dto!.Id);
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnStatusCodeOk_IfRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = _factory.SeedAndGetTestUser();
        var token = _factory.GetToken(user!);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // Act
        var createQuestionCommand = new CreateQuestionCommand()
        {
            Title = "Test Title",
            Content = "Test content"
        };
        var response = await client.PostAsJsonAsync("/questions", createQuestionCommand);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
    }
}