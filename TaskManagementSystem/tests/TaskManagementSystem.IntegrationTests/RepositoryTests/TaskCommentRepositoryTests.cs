using FluentAssertions;
using Moq;
using TaskManagementSystem.Dal.Models;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.IntegrationTests.Fakers;
using TaskManagementSystem.IntegrationTests.Fixtures;
using TaskManagementSystem.Utilities.Providers.Interfaces;
using Xunit;

namespace TaskManagementSystem.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskCommentRepositoryTests
{
    private readonly ITaskCommentRepository _repository;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderFake;

    public TaskCommentRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskCommentRepository;
        _dateTimeProviderFake = fixture.DateTimeProviderFake;
    }

    [Fact]
    public async Task Add_TaskComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate().First();
        
        // Act
        var result = await _repository.Add(taskComment, default);
        
        // Asserts
        result.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task GetComments_SingleTaskComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate().First();
        var taskCommentId = await _repository.Add(taskComment, default);
        var expectedTaskComment = taskComment
            .WithId(taskCommentId);
        var includeDeleted = false;
        
        // Act
        var results = await _repository.GetComments(new TaskCommentGetModel()
        {
            TaskId = taskComment.TaskId,
            IncludeDeleted = includeDeleted
        }, default);
        
        // Asserts
        results.Should().HaveCount(1);
        var actualTaskComment = results.Single();

        actualTaskComment.Should().BeEquivalentTo(expectedTaskComment);
    }

    [Fact]
    public async Task Update_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate().First();
        var taskCommentId = await _repository.Add(taskComment, default);
        var modifiedAt = DateTimeOffset.UtcNow;
        _dateTimeProviderFake.Setup(f => f.UtcNow()).Returns(modifiedAt);
        
        var updatedTaskComment = TaskCommentEntityV1Faker.Generate().First()
            .WithId(taskCommentId);
        
        var expectedTaskComment = updatedTaskComment
            .WithModifiedAt(modifiedAt);
        var includeDeleted = false;
        
        // Act
        await _repository.Update(updatedTaskComment, default);
        
        // Asserts
        var results = await _repository.GetComments(new TaskCommentGetModel()
        {
            TaskId = updatedTaskComment.TaskId,
            IncludeDeleted = includeDeleted
        }, default);
        results.Should().HaveCount(1);
        var actualTaskComment = results.Single();

        actualTaskComment.Should().BeEquivalentTo(expectedTaskComment);
    }
    
    [Fact]
    public async Task SetDeleted_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate().First();
        var taskCommentId = await _repository.Add(taskComment, default);
        var deletedAt = DateTimeOffset.UtcNow;
        _dateTimeProviderFake.Setup(f => f.UtcNow()).Returns(deletedAt);
        var expectedTaskComment = taskComment
            .WithId(taskCommentId)
            .WithDeletedAt(deletedAt);
        var includeDeleted = true;
        
        // Act
        await _repository.SetDeleted(taskCommentId, default);
        
        // Asserts
        var results = await _repository.GetComments(new TaskCommentGetModel()
        {
            TaskId = taskComment.TaskId,
            IncludeDeleted = includeDeleted
        }, default);
        results.Should().HaveCount(1);
        var actualTaskComment = results.Single();

        actualTaskComment.Should().BeEquivalentTo(expectedTaskComment);
    }
}