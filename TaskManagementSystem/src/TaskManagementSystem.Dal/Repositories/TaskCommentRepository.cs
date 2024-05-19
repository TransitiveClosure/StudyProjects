using Dapper;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.Dal.Settings;
using TaskManagementSystem.Utilities.Providers.Interfaces;

namespace TaskManagementSystem.Dal.Repositories;

public class TaskCommentRepository : PgRepository, ITaskCommentRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public TaskCommentRepository(
        IOptions<DalOptions> dalSettings, IDateTimeProvider dateTimeProvider) : base(dalSettings.Value)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<long> Add(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
   insert into task_comments (task_id, author_user_id, message, at)
   select task_id, author_user_id, message, at
     from UNNEST(@TaskComment)
returning id;
";
        await using var connection = await GetConnection();
        var id = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskComment = new TaskCommentEntityV1[] {model}
                },
                cancellationToken: token));
        return id.First();
    }

    public async Task Update(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments
   set task_id =        @TaskId,
       author_user_id = @AuthorUserId,
       message =        @Message,
       at =             @At,
       modified_at =    @ModifiedAt
 where id = @TaskCommentId;
";
        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AuthorUserId = model.AuthorUserId,
                    Message = model.Message,
                    At = model.At,
                    ModifiedAt = _dateTimeProvider.UtcNow(),
                    TaskCommentId = model.Id
                },
                cancellationToken: token));
    }

    public async Task SetDeleted(long commentId, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments
   set deleted_at = @DeletedAt
 where id = @TaskCommentId;
";
        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    DeletedAt = _dateTimeProvider.UtcNow(),
                    TaskCommentId = commentId
                },
                cancellationToken: token));
    }

    public async Task<TaskCommentEntityV1[]> GetComments(TaskCommentGetModel model, CancellationToken token)
    {
        string baseSql = @"
select id
     , task_id
     , author_user_id
     , message
     , at
     , modified_at
     , deleted_at
  from task_comments";

        var conditions = new List<string>();
        var @params = new DynamicParameters();
        conditions.Add("task_id = @TaskId");
        @params.Add($"TaskId", model.TaskId);

        if (!model.IncludeDeleted)
        {
            conditions.Add("deleted_at is null");
        }

        var cmd = new CommandDefinition(
            baseSql + $" WHERE {string.Join(" AND ", conditions)} " +
            $"ORDER BY at desc",
            @params,
            cancellationToken: token
        );

        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskCommentEntityV1>(cmd))
            .ToArray();
    }

    public async Task<TaskCommentEntityV1?> GetCommentById(long commentId, CancellationToken token)
    {
        const string sqlQuery = @"
select id
     , task_id
     , author_user_id
     , message
     , at
     , modified_at
     , deleted_at
  from task_comments
 where id = @TaskCommentId;
";
        await using var connection = await GetConnection();
        var comment = (await connection.QueryAsync<TaskCommentEntityV1>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskCommentId = commentId
                },
                cancellationToken: token))).SingleOrDefault();
        return comment;
    }
}