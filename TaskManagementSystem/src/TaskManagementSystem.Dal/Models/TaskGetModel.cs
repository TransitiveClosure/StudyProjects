namespace TaskManagementSystem.Dal.Models;

public record TaskGetModel
{
    public required long[] TaskIds { get; init; }
}