using ConsoleWord;
using ConsoleWord.Application.Commands;
using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Dropbox;
using ConsoleWord.Application.Helpers;
using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// Эта директива приводит ICloudStorageProvider к типу из Dropbox


class Program
{
    
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                string notificationFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notifications.txt");

                // Регистрация зависимости для пути к файлу уведомлений
                services.AddSingleton(notificationFile);
                services.AddSingleton<NotificationService>();

                // Регистрация хранилища: регистрируем IStorageProvider и ICloudStorageProvider
                services.AddSingleton<IStorageProvider, LocalFileStorageProvider>();
                // Вместо CloudFileStorage, регистрируем DropboxStorageProvider, который реализует ICloudStorageProvider из Dropbox
                services.AddSingleton<ICloudDropBoxStorageProvider>(provider => 
                {
                    // Create a DropboxClient using the token
                    //var dropboxClient = new DropboxClient(dropboxToken);
                    return new DropboxStorageProvider(); // Pass the DropboxClient instance to DropboxStorageProvider
                });

                // Регистрация других сервисов приложения
                services.AddSingleton<StorageService>();
                services.AddScoped<IDocumentCommand, DeleteDocumentCommand>();
                services.AddSingleton<UndoRedoService>();
                services.AddSingleton<AuthenticationService>();


                
                services.AddSingleton<FormattingService>();
                services.AddSingleton<CloudFileStorage>();
                services.AddSingleton<DocumentEditor>();
                services.AddSingleton<DocumentFactory>();
                services.AddSingleton<DocumentStorageService>();
                services.AddSingleton<DocumentService>();
                
                services.AddScoped<NotifySubscribersHelper>();
                services.AddScoped<ShowAuthenticationOptionsHepler>();
                services.AddScoped<PermissionManager>();
                services.AddScoped<Menu>();


                // Регистрация Hangfire
                services.AddHangfire(config => config.UseMemoryStorage());
                services.AddHangfireServer();
            })
            .Build();

        // Получение и отображение меню
        var menu = host.Services.GetRequiredService<Menu>();
        menu.Show();

        // Работа с уведомлениями
        var notifier = host.Services.GetRequiredService<NotificationService>();
        notifier.SendNotification("admin", "Test message");

        Console.WriteLine("Notification sent. Waiting for background job to complete...");
        System.Threading.Thread.Sleep(5000);

        var userNotifications = notifier.GetUserNotifications("admin");
        Console.WriteLine("\nNotifications for user 'admin':");
        foreach (var notification in userNotifications)
        {
            Console.WriteLine(notification);
        }

        Console.WriteLine("Check notifications.txt now.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}