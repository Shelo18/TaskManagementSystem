using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IValidator<CreateProjectDto> _createValidator;
    private readonly IValidator<UpdateProjectDto> _updateValidator;

    public ProjectsController(
        IProjectService projectService,
        IValidator<CreateProjectDto> createValidator,
        IValidator<UpdateProjectDto> updateValidator)
    {
        _projectService = projectService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Get all projects with pagination, filtering and sorting</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProjectDto>>> GetAll([FromQuery] QueryParameters parameters)
    {
        var result = await _projectService.GetAllAsync(parameters);
        return Ok(result);
    }

    /// <summary>Get a single project by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project is null)
            return NotFound(new { message = $"Project with ID {id} was not found." });
        return Ok(project);
    }

    /// <summary>Create a new project</summary>
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var created = await _projectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an existing project</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] UpdateProjectDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var updated = await _projectService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"Project with ID {id} was not found." });
        return Ok(updated);
    }

    /// <summary>Delete a project</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _projectService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Project with ID {id} was not found." });
        return NoContent();
    }
}
