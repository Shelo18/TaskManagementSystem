using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }

    public async Task<Project?> GetWithTasksAsync(int id)
        => await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<PagedResult<Project>> GetPagedAsync(QueryParameters parameters)
    {
        var query = _context.Projects.Include(p => p.Tasks).AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                (p.Description != null && p.Description.ToLower().Contains(search)));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "name" => parameters.SortDirection == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            _ => query.OrderBy(p => p.Id)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Project>
        {
            Items = items,
            TotalCount = total,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
