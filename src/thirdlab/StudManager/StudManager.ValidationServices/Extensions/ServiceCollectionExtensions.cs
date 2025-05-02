using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StudManager.Application.DTOs;
using StudManager.Application.ValidationInterfaces;
using StudManager.ValidationServices.ValidateRules;
using StudManager.ValidationServices.ValidationServices;

namespace StudManager.ValidationServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureValidationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<StudentDTO>, StudentValidator>();

        services.AddScoped<IStudentValidationService, StudentValidationService>();
    }
}