using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;

namespace TaskManagementSystem.Dal.Repositories.Interfaces;

public interface ITaskCommentRepository
{
    Task<long> Add(TaskCommentEntityV1 model, CancellationToken token);
    Task Update(TaskCommentEntityV1 model, CancellationToken token);
    Task SetDeleted(long taskId, CancellationToken token);
    Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token);
}