using AutoBogus;
using Bogus;
using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.IntegrationTests.Creators;

namespace TaskManagementSystem.IntegrationTests.Fakers;

public static class TaskCommentEntityV1Faker
{
    private static readonly object Lock = new();

    private static readonly Faker<TaskCommentEntityV1> Faker = new AutoFaker<TaskCommentEntityV1>()
        .RuleFor(x => x.Id, _ => Create.RandomId())
        .RuleFor(x => x.TaskId, _ => Create.RandomId())
        .RuleFor(x => x.AuthorUserId, _ => Create.RandomId())
        .RuleFor(x => x.At, f => f.Date.RecentOffset().UtcDateTime)
        .RuleFor(x => x.ModifiedAt, _ => null)
        .RuleFor(x => x.DeletedAt, _ => null);

    public static TaskCommentEntityV1[] Generate(int count = 1)
    {
        lock (Lock)
        {
            return Faker.Generate(count).ToArray();
        }
    }
    
    public static TaskCommentEntityV1 WithId(
        this TaskCommentEntityV1 src, 
        long id)
        => src with { Id = id };
    
    public static TaskCommentEntityV1 WithModifiedAt(
        this TaskCommentEntityV1 src, 
        DateTimeOffset modifiedAt)
        => src with { ModifiedAt = modifiedAt };
    
    public static TaskCommentEntityV1 WithDeletedAt(
        this TaskCommentEntityV1 src, 
        DateTimeOffset deletedAt)
        => src with { DeletedAt = deletedAt };
}