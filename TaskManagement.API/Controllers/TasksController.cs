using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskItemService _taskService;
    private readonly IValidator<CreateTaskItemDto> _createValidator;
    private readonly IValidator<UpdateTaskItemDto> _updateValidator;

    public TasksController(
        ITaskItemService taskService,
        IValidator<CreateTaskItemDto> createValidator,
        IValidator<UpdateTaskItemDto> updateValidator)
    {
        _taskService = taskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Get all tasks with pagination, filtering (by projectId, userId, status) and sorting</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskItemDto>>> GetAll([FromQuery] TaskQueryParameters parameters)
    {
        var result = await _taskService.GetAllAsync(parameters);
        return Ok(result);
    }

    /// <summary>Get a single task by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItemDto>> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task is null)
            return NotFound(new { message = $"Task with ID {id} was not found." });
        return Ok(task);
    }

    /// <summary>Create a new task</summary>
    [HttpPost]
    public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskItemDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var created = await _taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an existing task</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItemDto>> Update(int id, [FromBody] UpdateTaskItemDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var updated = await _taskService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"Task with ID {id} was not found." });
        return Ok(updated);
    }

    /// <summary>Delete a task</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Task with ID {id} was not found." });
        return NoContent();
    }
}
