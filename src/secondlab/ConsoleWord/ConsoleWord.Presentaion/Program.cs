using ConsoleWord;
using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Storage;

class Program
{
    static void Main(string[] args)
    {
        // Создание сервисов
        StorageService storageService = new StorageService(new LocalFileStorage(), new CloudFileStorage());
        FormattingService formattingService = new FormattingService();
        DocumentService documentService = new DocumentService(storageService);
        Menu menu = new Menu(documentService);

        // Инициализация фабрики зависимостей
        DependencyFactory.Initialize(storageService, formattingService, menu);

        // Запуск меню
        menu.Show();
    }
}