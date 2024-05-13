namespace TaskManagementSystem.Dal.Models;

public record UserGetModel
{
    public required long[] UserIds { get; init; }
}