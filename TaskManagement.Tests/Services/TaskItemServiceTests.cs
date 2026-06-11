using AutoMapper;
using Moq;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Mappings;
using TaskManagement.Infrastructure.Services;
using Xunit;

namespace TaskManagement.Tests.Services;

public class TaskItemServiceTests
{
    private readonly Mock<ITaskItemRepository> _taskRepoMock;
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly IMapper _mapper;
    private readonly TaskItemService _service;

    public TaskItemServiceTests()
    {
        _taskRepoMock = new Mock<ITaskItemRepository>();
        _projectRepoMock = new Mock<IProjectRepository>();
        _userRepoMock = new Mock<IUserRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new TaskItemService(
            _taskRepoMock.Object,
            _projectRepoMock.Object,
            _userRepoMock.Object,
            _mapper);
    }

    private static TaskItem MakeTask(int id = 1) => new()
    {
        Id = id,
        Title = "Test Task",
        Status = TaskItemStatus.Todo,
        ProjectId = 1,
        Project = new Project { Id = 1, Name = "P" },
        AssignedUserId = null,
        Comments = new List<Comment>()
    };


    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedTask()
    {
        var dto = new CreateTaskItemDto { Title = "My Task", ProjectId = 1 };
        var entity = MakeTask();

        _projectRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync(entity);
        _taskRepoMock.Setup(r => r.GetWithDetailsAsync(entity.Id)).ReturnsAsync(entity);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
    }

    [Fact]
    public async Task CreateAsync_ProjectNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new CreateTaskItemDto { Title = "T", ProjectId = 99 };
        _projectRepoMock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new CreateTaskItemDto { Title = "T", ProjectId = 1, AssignedUserId = 55 };
        _projectRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _userRepoMock.Setup(r => r.ExistsAsync(55)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(dto));
    }


    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTask()
    {
        var task = MakeTask(7);
        _taskRepoMock.Setup(r => r.GetWithDetailsAsync(7)).ReturnsAsync(task);

        var result = await _service.GetByIdAsync(7);

        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _taskRepoMock.Setup(r => r.GetWithDetailsAsync(999)).ReturnsAsync((TaskItem?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var tasks = new List<TaskItem> { MakeTask(1), MakeTask(2) };
        var paged = new PagedResult<TaskItem> { Items = tasks, TotalCount = 2, Page = 1, PageSize = 10 };
        _taskRepoMock.Setup(r => r.GetPagedAsync(It.IsAny<TaskQueryParameters>())).ReturnsAsync(paged);

        var result = await _service.GetAllAsync(new TaskQueryParameters());

        Assert.Equal(2, result.TotalCount);
    }


    [Fact]
    public async Task UpdateAsync_ExistingTask_ReturnsUpdated()
    {
        var task = MakeTask(3);
        var dto = new UpdateTaskItemDto { Title = "Updated", ProjectId = 1, Status = TaskItemStatus.InProgress };
        var updated = MakeTask(3);
        updated.Title = "Updated";
        updated.Status = TaskItemStatus.InProgress;

        _taskRepoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(task);
        _projectRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
        _taskRepoMock.Setup(r => r.GetWithDetailsAsync(3)).ReturnsAsync(updated);

        var result = await _service.UpdateAsync(3, dto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result.Title);
        Assert.Equal(TaskItemStatus.InProgress, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingTask_ReturnsNull()
    {
        _taskRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        var result = await _service.UpdateAsync(999, new UpdateTaskItemDto { Title = "X", ProjectId = 1 });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_InvalidProject_ThrowsKeyNotFoundException()
    {
        var task = MakeTask(1);
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _projectRepoMock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(1, new UpdateTaskItemDto { Title = "X", ProjectId = 99 }));
    }


    [Fact]
    public async Task DeleteAsync_ExistingTask_ReturnsTrue()
    {
        var task = MakeTask(1);
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.DeleteAsync(task)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingTask_ReturnsFalse()
    {
        _taskRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }
}
