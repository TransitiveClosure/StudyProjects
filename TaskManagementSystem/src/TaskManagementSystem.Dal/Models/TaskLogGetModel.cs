namespace TaskManagementSystem.Dal.Models;

public record TaskLogGetModel
{
    public required long[] TaskIds { get; init; }
}