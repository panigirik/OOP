using Microsoft.Extensions.DependencyInjection;
using StudManager.Application.Adapters;
using StudManager.Application.Commands;
using StudManager.Application.Creators;
using StudManager.Application.Interfaces;
using StudManager.Application.Mappings;
using StudManager.Application.Services;

namespace StudManager.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCoreApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StudentMappingProfile));
        
        services.AddScoped<IAddStudentAsyncCommand, AddStudentAsyncCommand>();
        services.AddScoped<IGetAllStudentsAsyncCommand, GetAllStudentsAsyncCommand>();
        services.AddScoped<IEditStudentAsyncCommand, EditStudentAsyncCommand>();
        services.AddScoped<IQuoteService, QuoteAdapter>();
        services.AddSingleton<StudentCreator, DefaultStudentFactory>();
        services.AddScoped<IExternalQuoteApi, ExternalQuoteApi>();
        services.AddScoped<IQuoteService, QuoteService>();
    }
}