using AutoMapper;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public TaskItemService(
        ITaskItemRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<TaskItemDto>> GetAllAsync(TaskQueryParameters parameters)
    {
        var result = await _taskRepository.GetPagedAsync(parameters);
        return new PagedResult<TaskItemDto>
        {
            Items = _mapper.Map<IEnumerable<TaskItemDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<TaskItemDto?> GetByIdAsync(int id)
    {
        var task = await _taskRepository.GetWithDetailsAsync(id);
        return task is null ? null : _mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto)
    {
        var projectExists = await _projectRepository.ExistsAsync(dto.ProjectId);
        if (!projectExists)
            throw new KeyNotFoundException($"Project with ID {dto.ProjectId} was not found.");

        if (dto.AssignedUserId.HasValue)
        {
            var userExists = await _userRepository.ExistsAsync(dto.AssignedUserId.Value);
            if (!userExists)
                throw new KeyNotFoundException($"User with ID {dto.AssignedUserId} was not found.");
        }

        var task = _mapper.Map<TaskItem>(dto);
        var created = await _taskRepository.AddAsync(task);
        var withDetails = await _taskRepository.GetWithDetailsAsync(created.Id);
        return _mapper.Map<TaskItemDto>(withDetails!);
    }

    public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null) return null;

        var projectExists = await _projectRepository.ExistsAsync(dto.ProjectId);
        if (!projectExists)
            throw new KeyNotFoundException($"Project with ID {dto.ProjectId} was not found.");

        if (dto.AssignedUserId.HasValue)
        {
            var userExists = await _userRepository.ExistsAsync(dto.AssignedUserId.Value);
            if (!userExists)
                throw new KeyNotFoundException($"User with ID {dto.AssignedUserId} was not found.");
        }

        _mapper.Map(dto, task);
        await _taskRepository.UpdateAsync(task);

        var withDetails = await _taskRepository.GetWithDetailsAsync(task.Id);
        return _mapper.Map<TaskItemDto>(withDetails!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null) return false;

        await _taskRepository.DeleteAsync(task);
        return true;
    }
}
