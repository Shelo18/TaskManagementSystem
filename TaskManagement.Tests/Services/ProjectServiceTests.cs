using AutoMapper;
using Moq;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Mappings;
using TaskManagement.Infrastructure.Services;
using Xunit;

namespace TaskManagement.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _repoMock;
    private readonly IMapper _mapper;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _repoMock = new Mock<IProjectRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _service = new ProjectService(_repoMock.Object, _mapper);
    }


    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedProject()
    {
        var dto = new CreateProjectDto { Name = "New Project", Description = "Desc" };
        var entity = new Project { Id = 1, Name = "New Project", Description = "Desc" };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Project>())).ReturnsAsync(entity);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("New Project", result.Name);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task CreateAsync_EmptyName_StillCallsRepository()
    {
        var dto = new CreateProjectDto { Name = "X" };
        var entity = new Project { Id = 10, Name = "X" };
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Project>())).ReturnsAsync(entity);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
    }


    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProject()
    {
        var project = new Project { Id = 2, Name = "Alpha", Tasks = new List<TaskItem>() };
        _repoMock.Setup(r => r.GetWithTasksAsync(2)).ReturnsAsync(project);

        var result = await _service.GetByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Alpha", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetWithTasksAsync(999)).ReturnsAsync((Project?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, Name = "P1", Tasks = new List<TaskItem>() },
            new() { Id = 2, Name = "P2", Tasks = new List<TaskItem>() }
        };
        var paged = new PagedResult<Project> { Items = projects, TotalCount = 2, Page = 1, PageSize = 10 };
        _repoMock.Setup(r => r.GetPagedAsync(It.IsAny<QueryParameters>())).ReturnsAsync(paged);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }


    [Fact]
    public async Task UpdateAsync_ExistingProject_ReturnsUpdated()
    {
        var project = new Project { Id = 1, Name = "Old Name" };
        var dto = new UpdateProjectDto { Name = "New Name", Description = "Updated" };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingProject_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        var result = await _service.UpdateAsync(999, new UpdateProjectDto { Name = "X" });

        Assert.Null(result);
    }


    [Fact]
    public async Task DeleteAsync_ExistingProject_ReturnsTrue()
    {
        var project = new Project { Id = 1, Name = "P" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _repoMock.Setup(r => r.DeleteAsync(project)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingProject_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }
}
