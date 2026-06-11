using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Users.Where(u => u.Email == email);
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<PagedResult<User>> GetPagedAsync(QueryParameters parameters)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search));
        }

        query = parameters.SortBy?.ToLower() switch
        {
            "firstname" => parameters.SortDirection == "desc" ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => parameters.SortDirection == "desc" ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "email" => parameters.SortDirection == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            _ => query.OrderBy(u => u.Id)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = total,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
