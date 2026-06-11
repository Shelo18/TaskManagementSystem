using AutoMapper;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(QueryParameters parameters)
    {
        var result = await _userRepository.GetPagedAsync(parameters);
        return new PagedResult<UserDto>
        {
            Items = _mapper.Map<IEnumerable<UserDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");

        var user = _mapper.Map<User>(dto);
        var created = await _userRepository.AddAsync(user);
        return _mapper.Map<UserDto>(created);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return null;

        if (await _userRepository.EmailExistsAsync(dto.Email, id))
            throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");

        _mapper.Map(dto, user);
        await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return false;

        await _userRepository.DeleteAsync(user);
        return true;
    }
}
