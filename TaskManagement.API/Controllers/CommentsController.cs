using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs.Comment;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IValidator<CreateCommentDto> _createValidator;
    private readonly IValidator<UpdateCommentDto> _updateValidator;

    public CommentsController(
        ICommentService commentService,
        IValidator<CreateCommentDto> createValidator,
        IValidator<UpdateCommentDto> updateValidator)
    {
        _commentService = commentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Get all comments with pagination</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<CommentDto>>> GetAll([FromQuery] QueryParameters parameters)
    {
        var result = await _commentService.GetAllAsync(parameters);
        return Ok(result);
    }

    /// <summary>Get a single comment by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetById(int id)
    {
        var comment = await _commentService.GetByIdAsync(id);
        if (comment is null)
            return NotFound(new { message = $"Comment with ID {id} was not found." });
        return Ok(comment);
    }

    /// <summary>Get all comments for a specific task</summary>
    [HttpGet("task/{taskId:int}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetByTask(int taskId)
    {
        var comments = await _commentService.GetByTaskIdAsync(taskId);
        return Ok(comments);
    }

    /// <summary>Create a new comment</summary>
    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var created = await _commentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an existing comment</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> Update(int id, [FromBody] UpdateCommentDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var updated = await _commentService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"Comment with ID {id} was not found." });
        return Ok(updated);
    }

    /// <summary>Delete a comment</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _commentService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Comment with ID {id} was not found." });
        return NoContent();
    }
}
