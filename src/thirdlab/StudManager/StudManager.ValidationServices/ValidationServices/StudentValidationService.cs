using FluentValidation;
using FluentValidation.Results;
using StudManager.Application.DTOs;
using StudManager.Application.ValidationInterfaces;

namespace StudManager.ValidationServices.ValidationServices;

public class StudentValidationService: IStudentValidationService
{
    private readonly IValidator<StudentDTO> _validator;

    public StudentValidationService(IValidator<StudentDTO> validator)
    {
        _validator = validator;
    }
    
    public async Task<ValidationResult> ValidateLoginRequestAsync(StudentDTO request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        
        return result;
    }
    
}