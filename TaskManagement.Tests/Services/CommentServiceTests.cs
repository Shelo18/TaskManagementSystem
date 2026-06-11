using AutoMapper;
using Moq;
using TaskManagement.Core.DTOs.Comment;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Mappings;
using TaskManagement.Infrastructure.Services;
using Xunit;

namespace TaskManagement.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _commentRepoMock;
    private readonly Mock<ITaskItemRepository> _taskRepoMock;
    private readonly IMapper _mapper;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _commentRepoMock = new Mock<ICommentRepository>();
        _taskRepoMock = new Mock<ITaskItemRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new CommentService(_commentRepoMock.Object, _taskRepoMock.Object, _mapper);
    }

    private static Comment MakeComment(int id = 1, int taskId = 1) =>
        new() { Id = id, Content = "Test comment", TaskId = taskId };


    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedComment()
    {
        var dto = new CreateCommentDto { Content = "Great work!", TaskId = 1 };
        var entity = MakeComment();

        _taskRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _commentRepoMock.Setup(r => r.AddAsync(It.IsAny<Comment>())).ReturnsAsync(entity);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Test comment", result.Content);
    }

    [Fact]
    public async Task CreateAsync_TaskNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new CreateCommentDto { Content = "Hello", TaskId = 99 };
        _taskRepoMock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(dto));
    }


    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsComment()
    {
        var comment = MakeComment(5);
        _commentRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);

        var result = await _service.GetByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _commentRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Comment?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTaskIdAsync_ReturnsComments()
    {
        var comments = new List<Comment> { MakeComment(1, 3), MakeComment(2, 3) };
        _commentRepoMock.Setup(r => r.GetByTaskIdAsync(3)).ReturnsAsync(comments);

        var result = await _service.GetByTaskIdAsync(3);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var comments = new List<Comment> { MakeComment(1), MakeComment(2) };
        var paged = new PagedResult<Comment> { Items = comments, TotalCount = 2, Page = 1, PageSize = 10 };
        _commentRepoMock.Setup(r => r.GetPagedAsync(It.IsAny<QueryParameters>())).ReturnsAsync(paged);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Equal(2, result.TotalCount);
    }


    [Fact]
    public async Task UpdateAsync_ExistingComment_ReturnsUpdated()
    {
        var comment = MakeComment(2);
        var dto = new UpdateCommentDto { Content = "Updated content" };

        _commentRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(comment);
        _commentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(2, dto);

        Assert.NotNull(result);
        Assert.Equal("Updated content", result.Content);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingComment_ReturnsNull()
    {
        _commentRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Comment?)null);

        var result = await _service.UpdateAsync(999, new UpdateCommentDto { Content = "X" });

        Assert.Null(result);
    }


    [Fact]
    public async Task DeleteAsync_ExistingComment_ReturnsTrue()
    {
        var comment = MakeComment(1);
        _commentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);
        _commentRepoMock.Setup(r => r.DeleteAsync(comment)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingComment_ReturnsFalse()
    {
        _commentRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Comment?)null);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }
}
