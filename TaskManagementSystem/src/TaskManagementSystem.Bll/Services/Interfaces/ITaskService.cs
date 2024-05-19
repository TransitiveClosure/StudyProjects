using TaskManagementSystem.Bll.Models;
using TaskManagementSystem.Bll.Models.Comments;
using TaskManagementSystem.Bll.Models.Tasks;

namespace TaskManagementSystem.Bll.Services.Interfaces;

public interface ITaskService
{
    Task<long> CreateTask(CreateTaskModel model, CancellationToken token);

    Task<GetTaskModel?> GetTask(long taskId, CancellationToken token);

    Task AssignTask(AssignTaskModel model, CancellationToken token);

    Task SetParentTask(SetParentTaskModel model, CancellationToken token);
}