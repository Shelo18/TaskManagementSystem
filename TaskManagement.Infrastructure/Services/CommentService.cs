using AutoMapper;
using TaskManagement.Core.DTOs.Comment;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskItemRepository _taskRepository;
    private readonly IMapper _mapper;

    public CommentService(
        ICommentRepository commentRepository,
        ITaskItemRepository taskRepository,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<CommentDto>> GetAllAsync(QueryParameters parameters)
    {
        var result = await _commentRepository.GetPagedAsync(parameters);
        return new PagedResult<CommentDto>
        {
            Items = _mapper.Map<IEnumerable<CommentDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<CommentDto?> GetByIdAsync(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        return comment is null ? null : _mapper.Map<CommentDto>(comment);
    }

    public async Task<IEnumerable<CommentDto>> GetByTaskIdAsync(int taskId)
    {
        var comments = await _commentRepository.GetByTaskIdAsync(taskId);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
    {
        var taskExists = await _taskRepository.ExistsAsync(dto.TaskId);
        if (!taskExists)
            throw new KeyNotFoundException($"Task with ID {dto.TaskId} was not found.");

        var comment = _mapper.Map<Comment>(dto);
        var created = await _commentRepository.AddAsync(comment);
        return _mapper.Map<CommentDto>(created);
    }

    public async Task<CommentDto?> UpdateAsync(int id, UpdateCommentDto dto)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment is null) return null;

        _mapper.Map(dto, comment);
        await _commentRepository.UpdateAsync(comment);
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment is null) return false;

        await _commentRepository.DeleteAsync(comment);
        return true;
    }
}
