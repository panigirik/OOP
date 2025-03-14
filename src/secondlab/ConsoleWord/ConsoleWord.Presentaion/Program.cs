using ConsoleWord;
using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Storage;
using ConsoleWord.Presentation;

class Program
{
    static void Main(string[] args)
    {
        DependencyFactory.Initialize(
            new StorageService(new LocalFileStorage(), new CloudFileStorage()), 
            new FormattingService(), 
            new Menu()
        );

        EditorApp app = new EditorApp();
        app.Run();
    }
}