using FluentValidation;
using TaskManagementSystem.Bll.Models.Comments;

namespace TaskManagementSystem.Bll.Services.Interfaces;

public interface ITaskCommentService
{
    Task<long> CreateComment(CreateTaskCommentModel model, CancellationToken token);
    
    Task<CommentModel[]> GetComments(GetTaskCommentModel model, CancellationToken token);

    Task UpdateTaskComment(UpdateTaskCommentModel model, CancellationToken token);

    Task SetDeleted(SetTaskCommentDeletedModel model, CancellationToken token);
}