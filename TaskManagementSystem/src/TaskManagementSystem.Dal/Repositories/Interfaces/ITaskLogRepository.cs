using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;

namespace TaskManagementSystem.Dal.Repositories.Interfaces;

public interface ITaskLogRepository
{
    Task<long[]> Add(TaskLogEntityV1[] tasks, CancellationToken token);
    
    Task<TaskLogEntityV1[]> Get(TaskLogGetModel query, CancellationToken token);
}