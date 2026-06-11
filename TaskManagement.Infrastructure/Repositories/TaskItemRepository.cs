using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
{
    public TaskItemRepository(AppDbContext context) : base(context) { }

    public async Task<TaskItem?> GetWithDetailsAsync(int id)
        => await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<PagedResult<TaskItem>> GetPagedAsync(TaskQueryParameters parameters)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .AsQueryable();

        if (parameters.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == parameters.ProjectId.Value);

        if (parameters.AssignedUserId.HasValue)
            query = query.Where(t => t.AssignedUserId == parameters.AssignedUserId.Value);

        if (parameters.Status.HasValue)
            query = query.Where(t => t.Status == parameters.Status.Value);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(search) ||
                (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "title" => parameters.SortDirection == "desc" ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            "status" => parameters.SortDirection == "desc" ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            "projectid" => parameters.SortDirection == "desc" ? query.OrderByDescending(t => t.ProjectId) : query.OrderBy(t => t.ProjectId),
            _ => query.OrderBy(t => t.Id)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<TaskItem>
        {
            Items = items,
            TotalCount = total,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
