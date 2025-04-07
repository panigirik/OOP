using ConsoleWord;
using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using ConsoleWord.Infrastracture.LocalStorage.Storage;
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
            })
            .Build();

        // Получаем сервис Menu из DI контейнера и показываем меню
        var menu = host.Services.GetRequiredService<Menu>();
        menu.Show();
    }
}