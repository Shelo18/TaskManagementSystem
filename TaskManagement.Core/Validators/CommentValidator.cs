using FluentValidation;
using TaskManagement.Core.DTOs.Comment;

namespace TaskManagement.Core.Validators;

public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");

        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("A valid TaskId is required.");
    }
}

public class UpdateCommentValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
    }
}
