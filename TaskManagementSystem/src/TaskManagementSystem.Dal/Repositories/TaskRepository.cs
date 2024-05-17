using Dapper;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.Dal.Settings;
using TaskStatus = TaskManagementSystem.Dal.Enums.TaskStatus;

namespace TaskManagementSystem.Dal.Repositories;

public class TaskRepository : PgRepository, ITaskRepository
{
    public TaskRepository(
        IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<long[]> Add(TaskEntityV1[] tasks, CancellationToken token)
    {
        const string sqlQuery = @"
insert into tasks (parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at) 
select parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at
  from UNNEST(@Tasks)
returning id;
";

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Tasks = tasks
                },
                cancellationToken: token));
        
        return ids
            .ToArray();
    }

    public async Task<TaskEntityV1[]> Get(TaskGetModel query, CancellationToken token)
    {
        var baseSql = @"
select id
     , parent_task_id
     , number
     , title
     , description
     , status
     , created_at
     , created_by_user_id
     , assigned_to_user_id
     , completed_at
  from tasks
";
        
        var conditions = new List<string>();
        var @params = new DynamicParameters();

        if (query.TaskIds.Any())
        {
            conditions.Add($"id = ANY(@TaskIds)");
            @params.Add($"TaskIds", query.TaskIds);
        }
        
        var cmd = new CommandDefinition(
            baseSql + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskEntityV1>(cmd))
            .ToArray();
    }

    public async Task Assign(AssignTaskModel model, CancellationToken token)
    {
        const string sqlQuery = @"
update tasks
   set assigned_to_user_id = @AssignToUserId
     , status = @Status
 where id = @TaskId
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AssignToUserId = model.AssignToUserId,
                    Status = model.Status
                },
                cancellationToken: token));
    }

    public async Task<SubTaskModel[]> GetSubTasksInStatus(long parentTaskId, TaskStatus[] statuses, CancellationToken token)
    {
        var baseSql = @"
 with recursive task_parents
   as (select t.id                           as task_id
            , t.title                        as title
            , t.status                       as status_id
            , '{' || t.parent_task_id::text  as parent_task_ids
         from tasks t
        where t.parent_task_id = @ParentTaskId
        union all
       select t.id                                     as task_id
            , t.title                                  as title
            , t.status                                 as status_id
            , tp.parent_task_ids || ', ' || tp.task_id as parent_task_ids
         from task_parents tp
         join tasks t on t.parent_task_id = tp.task_id
       )

select tp.task_id                               as task_id
     , tp.title                                 as title
     , ts.alias                                 as status
     , (tp.parent_task_ids || ' }') :: bigint[] as parent_task_ids
  from task_parents tp
  join task_statuses ts on ts.id = tp.status_id
        ";

        var conditions = new List<string>();
        var @params = new DynamicParameters();
        
        @params.Add($"ParentTaskId", parentTaskId);
        if (statuses.Any())
        {
            conditions.Add($"ts.alias = ANY(@TaskStatuses)");
            @params.Add($"TaskStatuses", statuses.Select(status => status.ToString()).ToList());
        }

        var cmd = new CommandDefinition(
            baseSql + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();

        var subTasks = (
            await connection.QueryAsync<SubTaskModel>(cmd)).ToArray();
        return subTasks;
    }

    public async Task SetParentTask(long taskId, long parentTaskId, CancellationToken token)
    {
        const string sqlQuery = @"
update tasks
   set parent_task_id = @ParentTaskId
 where id = @TaskId
";
        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = taskId,
                    ParentTaskId = parentTaskId
                },
                cancellationToken: token));
    }
}