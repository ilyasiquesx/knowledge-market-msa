using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Forum.Core.Entities.Questions;
using Forum.Core.Entities.Questions.Commands.Create;
using Forum.Core.Entities.Questions.Queries.GetById;
using Forum.IntegrationTests.Factories;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Forum.IntegrationTests;

public class QuestionsIntegrationTests
{
    private readonly Question _testQuestion;
    private readonly HttpClient _testClient;

    public QuestionsIntegrationTests()
    {
        var factory = new ForumWebApplicationFactory();
        _testClient = factory.CreateClient();

        _testQuestion = factory.SeedAndGetTestQuestion() ?? throw new ArgumentNullException(nameof(_testQuestion));

        var user = factory.SeedAndGetTestUser() ?? throw new ArgumentNullException($"testUser");
        var token = factory.GetToken(user);

        _testClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    [Fact]
    public async Task GetQuestionById_ShouldReturnQuestion_IfQuestionExists()
    {
        // Arrange
        var questionUrl = $"/questions/{_testQuestion.Id}";

        // Act
        var response = await _testClient.GetAsync(questionUrl);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<QuestionDto>();
        Assert.Equal(_testQuestion.Id, dto!.Id);
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnStatusCodeOk_IfRequestIsValid()
    {
        // Arrange
        var createQuestionCommand = new CreateQuestionCommand
        {
            Title = "Test Title",
            Content = "Test content"
        };

        // Act
        var response = await _testClient.PostAsJsonAsync("/questions", createQuestionCommand);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnBadRequest_IfContentIsEmpty()
    {
        // Arrange
        var createQuestionCommand = new CreateQuestionCommand
        {
            Content = string.Empty,
            Title = "Not empty title"
        };

        // Act
        var response = await _testClient.PostAsJsonAsync("/questions", createQuestionCommand);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }
}