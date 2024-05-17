using FluentAssertions;
using TaskManagementSystem.Dal.Models;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.IntegrationTests.Creators;
using TaskManagementSystem.IntegrationTests.Fakers;
using TaskManagementSystem.IntegrationTests.Fixtures;
using TaskManagementSystem.IntegrationTests.Generators;
using Xunit;
using TaskStatus = TaskManagementSystem.Dal.Enums.TaskStatus;

namespace TaskManagementSystem.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);
        
        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }
    
    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);
        
        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }
    
    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();
        
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);
        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);
        
        // Act
        await _repository.Assign(assign, default);
        
        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        results.Should().HaveCount(1);
        var task = results.Single();
        
        expectedTask = expectedTask with {Status = assign.Status};
        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task SetParentTask_SetOneTask_Success()
    {
        // Arrange
        var parentTaskId = Create.RandomId();
        
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithParentTaskId(parentTaskId);
        
        // Act
        await _repository.SetParentTask(expectedTaskId, parentTaskId, default);
        
        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        results.Should().HaveCount(1);
        var task = results.Single();
        
        task.Should().BeEquivalentTo(expectedTask);
    }
    
    [Fact]
    public async Task GetSubTasksInStatus_GetALotOfSubtasks_Success()
    {
        // Arrange
        const int numberOfTasks = 20;
        const int maxChildrenNumber = 2;
        var statuses = new TaskStatus[] {TaskStatus.InProgress, TaskStatus.Done };
        var tasksList = TaskEntityV1Faker.Generate(numberOfTasks).ToList();
        var rand = new Random();
        tasksList = tasksList.Select(task =>
            task.WithStatus((int)statuses[rand.Next(0, statuses.Length)])).ToList();
        
        var taskIds = await _repository.Add(tasksList.ToArray(), default);
        var parentTaskId = taskIds.First();
        
        for (var taskNumber = 0; taskNumber < tasksList.Count; taskNumber++)
        {
            tasksList[taskNumber] = tasksList[taskNumber]
                .WithId(taskIds[taskNumber]);
        }
        var expectedSubTasks = HierarchicalTaskGenerator.CreateTaskHierarchy(tasksList.ToArray(), maxChildrenNumber);
        await Parallel.ForEachAsync(
            expectedSubTasks.ToList(),
            async (subTask, _) => await _repository.SetParentTask(subTask.TaskId, subTask.ParentTaskIds.Last(), default));
        
        // Act
        var results = await _repository.GetSubTasksInStatus(parentTaskId,statuses, default);
        
        // Asserts
        results.Should().HaveCount(expectedSubTasks.Length);
        results.Should().BeEquivalentTo(expectedSubTasks);
    }
    
}
