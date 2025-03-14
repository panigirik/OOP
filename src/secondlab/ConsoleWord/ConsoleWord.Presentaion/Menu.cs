using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.LocalStorage.Storage;

namespace ConsoleWord;

public class Menu
{
    private CommandService _commandService = new CommandService();
    private Document _document;

    public void Show()
    {
        while (true)
        {
            Console.WriteLine("1. Create Document");
            Console.WriteLine("2. Insert Text");
            Console.WriteLine("3. Undo");
            Console.WriteLine("4. Redo");
            Console.WriteLine("5. Save Document");
            Console.WriteLine("6. Exit");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Enter document name: ");
                    string name = Console.ReadLine();
                    _document = new PlainTextDocument(name);
                    break;
                case "2":
                    if (_document != null)
                    {
                        Console.Write("Enter text: ");
                        string text = Console.ReadLine();
                        _commandService.ExecuteCommand(new InsertTextCommand(_document, text));
                    }
                    break;
                case "3":
                    _commandService.Undo();
                    break;
                case "4":
                    _commandService.Redo();
                    break;
                case "5":
                    if (_document != null)
                    {
                        Console.Write("Enter file path: ");
                        string path = Console.ReadLine();
                        new LocalFileStorage().Save(_document, path);
                    }
                    break;
                case "6":
                    return;
            }
        }
    }
}
