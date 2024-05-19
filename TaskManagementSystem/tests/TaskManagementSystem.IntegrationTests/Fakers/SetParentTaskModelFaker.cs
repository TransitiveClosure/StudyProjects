using AutoBogus;
using Bogus;
using TaskManagementSystem.Dal.Models;
using TaskManagementSystem.IntegrationTests.Creators;

namespace TaskManagementSystem.IntegrationTests.Fakers;

public static class SetParentTaskModelFaker
{
    private static readonly object Lock = new();

    private static readonly Faker<SetParentTaskModel> Faker = new AutoFaker<SetParentTaskModel>()
        .RuleFor(x => x.TaskId, _ => Create.RandomId())
        .RuleFor(x => x.ParentTaskId, _ => Create.RandomId());

    public static SetParentTaskModel[] Generate(int count = 1)
    {
        lock (Lock)
        {
            return Faker.Generate(count).ToArray();
        }
    }

    public static SetParentTaskModel WithTaskId(
        this SetParentTaskModel src, 
        long taskId)
        => src with { TaskId = taskId };
    
    public static SetParentTaskModel WithParentTaskId(
        this SetParentTaskModel src, 
        long parentTaskId)
        => src with { ParentTaskId = parentTaskId }; 
}