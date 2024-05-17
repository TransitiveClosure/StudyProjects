using TaskManagementSystem.Bll.Models;

namespace TaskManagementSystem.Bll.Services.Interfaces;

public interface ITaskService
{
    Task<long> CreateTask(CreateTaskModel model, CancellationToken token);

    Task<GetTaskModel?> GetTask(long taskId, CancellationToken token);

    Task AssignTask(Bll.Models.AssignTaskModel model, CancellationToken token);

    Task<TaskMessage[]> GetComments(long taskId, CancellationToken token);
}