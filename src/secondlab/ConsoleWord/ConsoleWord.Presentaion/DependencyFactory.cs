using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture;

namespace ConsoleWord;

public static class DependencyFactory
{
    private static StorageService _storageService;
    private static FormattingService _formattingService;
    private static Menu _menu;

    public static void Initialize(StorageService storageService, FormattingService formattingService, Menu menu)
    {
        _storageService = storageService;
        _formattingService = formattingService;
        _menu = menu;
    }

    public static StorageService GetStorageService() => _storageService;
    public static FormattingService GetFormattingService() => _formattingService;
    public static Menu GetMenu() => _menu;
}