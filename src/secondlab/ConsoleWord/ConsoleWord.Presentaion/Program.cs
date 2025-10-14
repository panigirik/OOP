using ConsoleWord;
using ConsoleWord.Application.Commands;
using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Dropbox;
using ConsoleWord.Application.Helpers;
using ConsoleWord.Application.Interfaces;
using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


class Program
{
    
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((services) =>
            {
                string notificationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notifications.txt");
                
                services.AddSingleton(notificationFile);
                services.AddSingleton<NotificationService>();
                
                services.AddSingleton<IStorageProvider, LocalFileStorageProvider>();
                services.AddSingleton<ICloudDropBoxStorageProvider>(provider => 
                {
                    return new DropboxStorageProvider(); 
                });
                
                services.AddSingleton<StorageService>();
                services.AddScoped<IDocumentCommand, DeleteDocumentCommand>();
                services.AddSingleton<UndoRedoService>();
                services.AddSingleton<AuthenticationService>();
                services.AddSingleton<EditorSettings>();
                services.AddSingleton<DocumentEditor>();
                services.AddSingleton<DocumentFactory>();
                services.AddSingleton<DocumentStorageService>();
                services.AddSingleton<DocumentService>();
                
                services.AddScoped<NotifySubscribersHelper>();
                services.AddScoped<SaveDocumentToCloudHelper>();
                services.AddScoped<ShowAuthenticationOptionsHepler>();
                services.AddScoped<PermissionManager>();
                services.AddScoped<Menu>();
                
                services.AddHangfire(config => config.UseMemoryStorage());
                services.AddHangfireServer();
            })
            .Build();
        
        var menu = host.Services.GetRequiredService<Menu>();
        menu.Show();
        

        Console.WriteLine("Check notifications.txt now.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}