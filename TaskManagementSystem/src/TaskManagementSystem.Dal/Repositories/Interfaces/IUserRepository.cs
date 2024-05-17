using TaskManagementSystem.Dal.Entities;
using TaskManagementSystem.Dal.Models;

namespace TaskManagementSystem.Dal.Repositories.Interfaces;

public interface IUserRepository
{
    Task<long[]> Add(UserEntityV1[] users, CancellationToken token);
    
    Task<UserEntityV1[]> Get(UserGetModel query, CancellationToken token);
}