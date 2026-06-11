using AutoMapper;
using Moq;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Mappings;
using TaskManagement.Infrastructure.Services;
using Xunit;

namespace TaskManagement.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly IMapper _mapper;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repoMock = new Mock<IUserRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _service = new UserService(_repoMock.Object, _mapper);
    }


    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedUser()
    {
        var dto = new CreateUserDto { FirstName = "Ana", LastName = "Gelashvili", Email = "ana@example.com" };
        var entity = new User { Id = 1, FirstName = "Ana", LastName = "Gelashvili", Email = "ana@example.com" };

        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(entity);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Ana", result.FirstName);
        Assert.Equal("ana@example.com", result.Email);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var dto = new CreateUserDto { FirstName = "Ana", LastName = "Gelashvili", Email = "existing@example.com" };

        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null)).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }


    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsUser()
    {
        var user = new User { Id = 5, FirstName = "Luka", LastName = "Tsiklauri", Email = "luka@example.com" };
        _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("Luka", result.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var users = new List<User>
        {
            new() { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com" },
            new() { Id = 2, FirstName = "C", LastName = "D", Email = "c@d.com" }
        };
        var paged = new PagedResult<User> { Items = users, TotalCount = 2, Page = 1, PageSize = 10 };
        _repoMock.Setup(r => r.GetPagedAsync(It.IsAny<QueryParameters>())).ReturnsAsync(paged);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }


    [Fact]
    public async Task UpdateAsync_ExistingUser_ReturnsUpdatedUser()
    {
        var user = new User { Id = 3, FirstName = "Old", LastName = "Name", Email = "old@example.com" };
        var dto = new UpdateUserDto { FirstName = "New", LastName = "Name", Email = "new@example.com" };

        _repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);
        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, 3)).ReturnsAsync(false);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(3, dto);

        Assert.NotNull(result);
        Assert.Equal("New", result.FirstName);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingUser_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        var result = await _service.UpdateAsync(999, new UpdateUserDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var user = new User { Id = 3, FirstName = "A", LastName = "B", Email = "old@example.com" };
        var dto = new UpdateUserDto { FirstName = "A", LastName = "B", Email = "taken@example.com" };

        _repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);
        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, 3)).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(3, dto));
    }


    [Fact]
    public async Task DeleteAsync_ExistingUser_ReturnsTrue()
    {
        var user = new User { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _repoMock.Setup(r => r.DeleteAsync(user)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }
}
