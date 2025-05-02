using System.Net;
using Microsoft.Extensions.DependencyInjection;
using StudManager.Application.Creators;
using StudManager.Application.Extensions;
using StudManager.Application.Interfaces;
using StudManager.Application.Services;
using StudManager.Persistance.Extensions;
using StudManager.ValidationServices.Extensions;

namespace StudManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // === ТОЛЬКО ДЛЯ РАЗРАБОТКИ: Игнорируем ошибки SSL-сертификатов ===
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            // ===============================================================

            var services = new ServiceCollection();

            // 1) Инфраструктурный слой: DbContext + репозиторий
            var connectionString = "Host=localhost;Port=5432;Database=Students;Username=postgres;Password=Volvos80";
            services.AddInfrastructureRepositoriesServices(connectionString);
            services.AddInfrastructureValidationServices();
            // 2) Прикладной слой: сервисы + AutoMapper
            services.AddCoreApplicationServices();

            // 3) HTTP-клиент для QuoteService с отключённой проверкой SSL
            services.AddHttpClient<IQuoteService, QuoteService>()
                    .ConfigurePrimaryHttpMessageHandler(() =>
                        new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        });

            // 4) Собираем провайдер и резолвим зависимости
            var serviceProvider = services.BuildServiceProvider();
            var studentCreator = serviceProvider.GetRequiredService<StudentCreator>();
            var getAllStudentsAsyncCommand = serviceProvider.GetRequiredService<IGetAllStudentsAsyncCommand>();
            var editStudentCommand = serviceProvider.GetRequiredService<IEditStudentAsyncCommand>();
            var QuoteService   = serviceProvider.GetRequiredService<IQuoteService>();
            
            // 5) Запускаем консольное меню
            var menu = new Menu(getAllStudentsAsyncCommand, editStudentCommand, studentCreator);
            await menu.RunAsync();

            // 6) После выхода из меню выводим мотивационную цитату
            var quote = await QuoteService.GetMotivationalQuoteAsync();
            if (quote != null)
            {
                Console.WriteLine($"\n💡 Motivation for today:\n\"{quote.Content}\" — {quote.Author}\n");
            }
            else
            {
                Console.WriteLine("\n[!] Could not fetch a quote at this time.");
            }
            
            Console.WriteLine("Student Management System terminated.");
        }
    }
}
