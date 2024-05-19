using System.Text.Json;
using System.Transactions;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagementSystem.Bll.Models.Comments;
using TaskManagementSystem.Bll.Services.Interfaces;
using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;
using TaskManagementSystem.Dal.Repositories.Interfaces;

namespace TaskManagementSystem.Bll.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ITaskCommentRepository _taskCommentRepository;
    private readonly IDistributedCache _distributedCache;

    public TaskCommentService(
        ITaskCommentRepository taskCommentRepository,
        IDistributedCache distributedCache)
    {
        _taskCommentRepository = taskCommentRepository;
        _distributedCache = distributedCache;
    }

    public async Task<long> CreateComment(
        CreateTaskCommentModel model,
        CancellationToken token)
    {
        var comment = new TaskCommentEntityV1
        {
            TaskId = model.TaskId,
            Message = model.Message,
            At = DateTimeOffset.UtcNow,
            AuthorUserId = model.AuthorUserId
        };

        return await _taskCommentRepository.Add(comment, token);
    }

    public async Task<CommentModel[]> GetComments(
        GetTaskCommentModel model,
        CancellationToken token)
    {
        const int cacheTimeoutSeconds = 5;
        const int cachedTaskMessagesNumber = 5;

        var cacheKey = $"cache_task_comments:{model.TaskId}";
        var cachedTaskMessages = await _distributedCache.GetStringAsync(cacheKey, token);
        if (!string.IsNullOrEmpty(cachedTaskMessages))
        {
            return JsonSerializer.Deserialize<CommentModel[]>(cachedTaskMessages) ?? new CommentModel[] { };
        }

        TaskCommentEntityV1[] taskComments = (await _taskCommentRepository.GetComments(new TaskCommentGetModel
        {
            TaskId = model.TaskId,
            IncludeDeleted = model.IncludeDeleted
        }, token));

        var result = taskComments.Select(taskComment =>
            new CommentModel()
            {
                Id = taskComment.Id,
                TaskId = taskComment.TaskId,
                IsDeleted = taskComment.DeletedAt is not null,
                Comment = taskComment.Message,
                At = taskComment.At
            }).ToArray();

        var taskMessagesJson = JsonSerializer.Serialize(
            result.Take(cachedTaskMessagesNumber));
        await _distributedCache.SetStringAsync(
            cacheKey,
            taskMessagesJson,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheTimeoutSeconds)
            },
            token);

        return result;
    }

    public async Task UpdateTaskComment(
        UpdateTaskCommentModel model,
        CancellationToken token)
    {
        using var transaction = CreateTransactionScope();
        var comment = await _taskCommentRepository.GetCommentById(model.CommentId, token);
        if (comment is null)
        {
            return;
        }

        var updatedComment = comment with { Message = model.Message };
        await _taskCommentRepository.Update(updatedComment, token);
        transaction.Complete();
    }

    public async Task SetDeleted(
        SetTaskCommentDeletedModel model,
        CancellationToken token)
    {
        await _taskCommentRepository.SetDeleted(model.CommentId, token);
    }

    private TransactionScope CreateTransactionScope(
        IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = level,
                Timeout = TimeSpan.FromSeconds(5)
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}