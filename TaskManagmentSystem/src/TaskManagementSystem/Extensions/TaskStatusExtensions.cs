using GrpcTaskStatus = TaskManagementSystem.Proto.Client.TaskStatus;
using BllTaskStatus = TaskManagementSystem.Bll.Enums.TaskStatus;

namespace TaskManagementSystem.Extensions;

public static class TaskStatusExtensions
{
    public static GrpcTaskStatus ToGrpc(this BllTaskStatus src)
    {
        return src switch
        {
            BllTaskStatus.Draft => GrpcTaskStatus.Draft,
            BllTaskStatus.ToDo => GrpcTaskStatus.ToDo,
            BllTaskStatus.InProgress => GrpcTaskStatus.InProgress,
            BllTaskStatus.Done => GrpcTaskStatus.Done,
            BllTaskStatus.Canceled => GrpcTaskStatus.Canceled,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };
    }
}