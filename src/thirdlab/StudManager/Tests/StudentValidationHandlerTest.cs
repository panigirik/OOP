using FluentValidation;
using FluentValidation.Results;
using Moq;
using StudManager.Application.DTOs;
using StudManager.ValidationServices.ValidationServices;
using Xunit;

namespace Tests
{
    public class StudentValidationHandlerTest
    {
        [Fact]
        public async Task ValidateLoginRequestAsync_InvalidDto_ThrowsValidationException()
        {
            // Arrange
            var dto = new StudentDTO { Name = "A", Grade = -1 };
            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name must be at least 2 chars"),
                new ValidationFailure("Grade", "Grade must be non-negative")
            };
            
            var validatorMock = new Mock<IValidator<StudentDTO>>();
            validatorMock
                .Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult(failures));
            
            var svc = new StudentValidationService(validatorMock.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => svc.ValidateLoginRequestAsync(dto)
            );
        }
        

    }
}
