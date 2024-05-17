using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;
using TaskStatus = TaskManagementSystem.Dal.Enums.TaskStatus;

namespace TaskManagementSystem.IntegrationTests.Generators;

public static class HierarchicalTaskGenerator
{
    public static SubTaskModel[] CreateTaskHierarchy(TaskEntityV1[] tasks, int maxChildrenNumber)
    {
        if (!tasks.Any())
        {
            throw new ArgumentException();
        }

        var subTasks = new List<SubTaskModel>();
        var parentTaskIndex = 0;

        subTasks.Add(new SubTaskModel()
        {
            TaskId = tasks.First().Id,
            Title = tasks.First().Title,
            Status = (TaskStatus)(tasks.First().Status),
            ParentTaskIds = new long[]{}
        });

        for (int taskIndex = 1; taskIndex < tasks.Length; taskIndex++)
        {
            subTasks.Add(new SubTaskModel()
            {
                TaskId = tasks[taskIndex].Id,
                Title = tasks[taskIndex].Title,
                Status = (TaskStatus)(tasks[taskIndex].Status),
                ParentTaskIds = subTasks[parentTaskIndex].ParentTaskIds
                    .Concat(new long[] { subTasks[parentTaskIndex].TaskId }).ToArray()
            });

            if (taskIndex % maxChildrenNumber == 0)
            {
                parentTaskIndex++;
            }
        }
        // Skip root task
        return subTasks.Skip(1).ToArray();
    }
}