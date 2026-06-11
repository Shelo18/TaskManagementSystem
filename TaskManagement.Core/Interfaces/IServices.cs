using TaskManagement.Core.DTOs.Comment;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.DTOs.User;

namespace TaskManagement.Core.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllAsync(QueryParameters parameters);
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IProjectService
{
    Task<PagedResult<ProjectDto>> GetAllAsync(QueryParameters parameters);
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ITaskItemService
{
    Task<PagedResult<TaskItemDto>> GetAllAsync(TaskQueryParameters parameters);
    Task<TaskItemDto?> GetByIdAsync(int id);
    Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto);
    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ICommentService
{
    Task<PagedResult<CommentDto>> GetAllAsync(QueryParameters parameters);
    Task<CommentDto?> GetByIdAsync(int id);
    Task<IEnumerable<CommentDto>> GetByTaskIdAsync(int taskId);
    Task<CommentDto> CreateAsync(CreateCommentDto dto);
    Task<CommentDto?> UpdateAsync(int id, UpdateCommentDto dto);
    Task<bool> DeleteAsync(int id);
}
