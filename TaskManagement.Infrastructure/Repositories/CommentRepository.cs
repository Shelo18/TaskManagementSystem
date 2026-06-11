using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId)
        => await _context.Comments
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.Id)
            .ToListAsync();

    public async Task<PagedResult<Comment>> GetPagedAsync(QueryParameters parameters)
    {
        var query = _context.Comments.Include(c => c.Task).AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(c => c.Content.ToLower().Contains(search));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "taskid" => parameters.SortDirection == "desc"
                ? query.OrderByDescending(c => c.TaskId)
                : query.OrderBy(c => c.TaskId),
            _ => query.OrderBy(c => c.Id)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Comment>
        {
            Items = items,
            TotalCount = total,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
