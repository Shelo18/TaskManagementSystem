using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<PagedResult<User>> GetPagedAsync(QueryParameters parameters);
}

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetWithTasksAsync(int id);
    Task<PagedResult<Project>> GetPagedAsync(QueryParameters parameters);
}

public interface ITaskItemRepository : IRepository<TaskItem>
{
    Task<TaskItem?> GetWithDetailsAsync(int id);
    Task<PagedResult<TaskItem>> GetPagedAsync(TaskQueryParameters parameters);
}

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId);
    Task<PagedResult<Comment>> GetPagedAsync(QueryParameters parameters);
}
