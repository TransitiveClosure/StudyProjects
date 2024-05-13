using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TaskManagementSystem.Bll.Models;
using TaskManagementSystem.Bll.Services.Interfaces;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Proto.Client;

namespace TaskManagementSystem.Services;

public class TasksService :  TaskManagementSystem.Proto.Client.TasksService.TasksServiceBase
{
    private readonly ITaskService _taskService;
    
    public TasksService(
        ITaskService taskService)
    {
        _taskService = taskService;
    }

    public override async Task<V1CreateTaskResponse> V1CreateTask(V1CreateTaskRequest request, ServerCallContext context)
    {
        var taskId = await _taskService.CreateTask(new CreateTaskModel
        {
            Title = request.Title,
            Description = request.Description,
            UserId = request.UserId
        }, context.CancellationToken);

        return new V1CreateTaskResponse()
        {
            TaskId = taskId
        };
    }

    public override async Task<V1GetTaskResponse> V1GetTask(V1GetTaskRequest request, ServerCallContext context)
    {
        var task = await _taskService.GetTask(
            request.TaskId,
            context.CancellationToken);
        
        return new V1GetTaskResponse
        {
            TaskId = task.TaskId,
            Title = task.Title,
            Description = task.Description,
            AssignedToUserId = task.AssignedToUserId,
            Status = task.Status.ToGrpc(),
            CreatedAt = task.CreatedAt.ToTimestamp()
        };
    }

    public override async Task<Empty> V1AssignTask(V1AssignTaskRequest request, ServerCallContext context)
    {
        await _taskService.AssignTask(new AssignTaskModel
        {
            TaskId = request.TaskId,
            AssignToUserId = request.AssigneeUserId,
            UserId = request.UserId
        }, context.CancellationToken);
        
        return new Empty();
    }
}