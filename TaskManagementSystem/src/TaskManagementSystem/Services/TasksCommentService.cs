using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TaskManagementSystem.Bll.Models.Comments;
using TaskManagementSystem.Bll.Services.Interfaces;
using TaskManagementSystem.Proto.Client;

namespace TaskManagementSystem.Services;

public class TasksCommentService : TaskManagementSystem.Proto.Client.TaskCommentsService.TaskCommentsServiceBase
{
    private readonly ITaskCommentService _taskCommentService;

    public TasksCommentService(
        ITaskCommentService taskCommentService)
    {
        _taskCommentService = taskCommentService;
    }

    public override async Task<V1CreateTaskCommentResponse> V1CreateComment(V1CreateTaskCommentRequest request,
        ServerCallContext context)
    {
        var commentId = await _taskCommentService.CreateComment(new CreateTaskCommentModel
        {
            AuthorUserId = request.AuthorUserId,
            Message = request.Message,
            TaskId = request.TaskId
        }, context.CancellationToken);

        return new V1CreateTaskCommentResponse()
        {
            CommentId = commentId
        };
    }

    public override async Task<V1GetTaskCommentsResponse> V1GetTaskComments(V1GetTaskCommentsRequest request,
        ServerCallContext context)
    {
        var comments = await _taskCommentService.GetComments(new GetTaskCommentModel
        {
            TaskId = request.TaskId,
            IncludeDeleted = request.IncludeDeleted
        }, context.CancellationToken);

        return new V1GetTaskCommentsResponse
        {
            TaskComments =
            {
                comments.Select(comment => new TaskComments()
                {
                    CommentId = comment.Id,
                    TaskId = comment.TaskId,
                    Message = comment.Comment,
                    IsDeleted = comment.IsDeleted,
                    CreatedAt = comment.At.ToTimestamp()
                })
            }
        };
    }

    public override async Task<Empty> V1UpdateTaskComment(V1UpdateTaskCommentRequest request, ServerCallContext context)
    {
        await _taskCommentService.UpdateTaskComment(new UpdateTaskCommentModel
        {
            CommentId = request.CommentId,
            Message = request.Message
        }, context.CancellationToken);
    
        return new Empty();
    }

    public override async Task<Empty> V1SetDeletedTaskComment(V1SetDeletedTaskCommentRequest request,
        ServerCallContext context)
    {
        await _taskCommentService.SetDeleted(new SetTaskCommentDeletedModel()
        {
            CommentId = request.CommentId
        }, context.CancellationToken);
        return new Empty();
    }
}