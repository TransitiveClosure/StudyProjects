using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;
using TaskStatus = TaskManagementSystem.Dal.Enums.TaskStatus;

namespace TaskManagementSystem.Dal.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<long[]> Add(TaskEntityV1[] tasks, CancellationToken token);
    
    Task<TaskEntityV1[]> Get(TaskGetModel query, CancellationToken token);

    Task Assign(AssignTaskModel model, CancellationToken token);
    
    Task<SubTaskModel[]> GetSubTasksInStatus(long parentTaskId, TaskStatus[] statuses, CancellationToken token);
    
    Task SetParentTask(SetParentTaskModel setParentTaskModel, CancellationToken token);
}