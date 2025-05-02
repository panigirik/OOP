using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudManager.Persistance.Data;
using StudManager.Persistance.Repositories;
using StudManger.Domain.Interfaces;

namespace StudManager.Persistance.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //private static string connectionString = "Host=localhost;Port=5432;Database=Students;Username=postgres;Password=Volvos80";
        
        public static void AddInfrastructureRepositoriesServices(this IServiceCollection services, string connectionString)
        {
            // Регистрируем DbContext с PostgreSQL
            services.AddDbContext<StudentDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // Репозиторий
            services.AddScoped<IStudentRepository, StudentRepository>();
        }
    }
}