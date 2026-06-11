using FluentValidation;
using TaskManagement.Core.DTOs.Task;

namespace TaskManagement.Core.Validators;

public class CreateTaskItemValidator : AbstractValidator<CreateTaskItemDto>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.");

        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("A valid ProjectId is required.");

        RuleFor(x => x.AssignedUserId)
            .GreaterThan(0).WithMessage("A valid AssignedUserId is required.")
            .When(x => x.AssignedUserId.HasValue);
    }
}

public class UpdateTaskItemValidator : AbstractValidator<UpdateTaskItemDto>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.");

        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("A valid ProjectId is required.");

        RuleFor(x => x.AssignedUserId)
            .GreaterThan(0).WithMessage("A valid AssignedUserId is required.")
            .When(x => x.AssignedUserId.HasValue);
    }
}
