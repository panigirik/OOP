using FluentValidation.Results;
using StudManager.Application.DTOs;

namespace StudManager.Application.ValidationInterfaces;

public interface IStudentValidationService
{
    Task<ValidationResult> ValidateLoginRequestAsync(StudentDTO request);
}