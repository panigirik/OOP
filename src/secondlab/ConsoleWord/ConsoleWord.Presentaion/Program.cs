using ConsoleWord;
using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using ConsoleWord.Application.Notifications;  // Добавить пространство имен для NotificationService
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static void Main(string[] args)
    {
        // Настройка DI контейнера для консольного приложения
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Регистрация сервисов в контейнере DI
                services.AddSingleton<IStorageProvider, LocalFileStorageProvider>();
                services.AddSingleton<ICloudStorageProvider, CloudFileStorage>();
                services.AddSingleton<StorageService>();
                services.AddSingleton<AuthenticationService>();
                services.AddSingleton<FormattingService>();
                services.AddSingleton<DocumentService>();
                services.AddSingleton<Menu>();

                // Регистрация Hangfire с in-memory хранилищем
                services.AddHangfire(config => config.UseMemoryStorage());

                // Регистрация сервера Hangfire для выполнения фоновых задач
                services.AddHangfireServer();

                // Регистрация NotificationService
                services.AddSingleton<NotificationService>();
            })
            .Build();

        // Получаем сервис Menu из DI контейнера
        var menu = host.Services.GetRequiredService<Menu>();

        // Отображаем меню
        menu.Show();

        // Запуск сервера Hangfire, сервер будет работать в фоне
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
