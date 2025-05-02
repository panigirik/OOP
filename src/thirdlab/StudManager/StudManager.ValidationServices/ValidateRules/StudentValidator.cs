using FluentValidation;
using StudManager.Application.DTOs;

namespace StudManager.ValidationServices.ValidateRules;

public class StudentValidator: AbstractValidator<StudentDTO>
{
    public StudentValidator()
    {
        RuleFor(x => x.Name).MinimumLength(2).WithMessage("name must be at least 2 characters long.");
        RuleFor(g => g.Grade).GreaterThan(0).LessThan(11).WithMessage("1-10 scale");
    }
}