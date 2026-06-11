using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs.Common;
using TaskManagement.Core.DTOs.User;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserDto> _createValidator;
    private readonly IValidator<UpdateUserDto> _updateValidator;

    public UsersController(
        IUserService userService,
        IValidator<CreateUserDto> createValidator,
        IValidator<UpdateUserDto> updateValidator)
    {
        _userService = userService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Get all users with pagination, filtering and sorting</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAll([FromQuery] QueryParameters parameters)
    {
        var result = await _userService.GetAllAsync(parameters);
        return Ok(result);
    }

    /// <summary>Get a single user by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with ID {id} was not found." });
        return Ok(user);
    }

    /// <summary>Create a new user</summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var created = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an existing user</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var updated = await _userService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"User with ID {id} was not found." });
        return Ok(updated);
    }

    /// <summary>Delete a user</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"User with ID {id} was not found." });
        return NoContent();
    }
}
