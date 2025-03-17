using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;

namespace ConsoleWord
{
    public class Menu
    {
        private readonly CommandService _commandService = new CommandService();
        private readonly DocumentService _documentService;
        private Document? _document;

        public Menu(DocumentService documentService)
        {
            _documentService = documentService;
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear(); // Clear the console for better UX
                Console.WriteLine("==== Menu ====");
                Console.WriteLine("1. Create Document");

                if (_document != null)
                {
                    Console.WriteLine("2. Insert Text");
                    Console.WriteLine("3. Format Bold");
                    Console.WriteLine("4. Undo");
                    Console.WriteLine("5. Redo");
                    Console.WriteLine("6. Save Document");
                }
                Console.WriteLine("7. Exit");
                Console.WriteLine("===============");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        CreateDocument();
                        break;

                    case "2":
                        InsertText();
                        break;

                    case "3":
                        FormatBold();
                        break;

                    case "4":
                        Undo();
                        break;

                    case "5":
                        Redo();
                        break;

                    case "6":
                        SaveDocument();
                        break;

                    case "7":
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        private void CreateDocument()
        {
            if (_document == null)
            {
                _documentService.CreateAndSaveDocument();
                _document = new Document("Untitled", "", "Arial", 12); // Initialize with some default values
                Console.WriteLine("Document created successfully.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("A document has already been created.");
                Console.ReadLine();
            }
        }

        private void InsertText()
        {
            if (_document != null)
            {
                Console.Write("Enter text: ");
                string text = Console.ReadLine();
                _commandService.ExecuteCommand(new InsertTextCommand(_document, text));
            }
            else
            {
                Console.WriteLine("No document selected.");
                Console.ReadLine();
            }
        }

        private void FormatBold()
        {
            if (_document != null)
            {
                _document.FormatBold();
                Console.WriteLine("Text formatted as bold.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No document selected.");
                Console.ReadLine();
            }
        }

        private void Undo()
        {
            _commandService.Undo();
            Console.WriteLine("Undo operation completed.");
            Console.ReadLine();
        }

        private void Redo()
        {
            _commandService.Redo();
            Console.WriteLine("Redo operation completed.");
            Console.ReadLine();
        }

        private void SaveDocument()
        {
            if (_document != null)
            {
                Console.Write("Enter save directory: ");
                string directory = Console.ReadLine()?.Trim();
                _documentService.SaveDocumentLocally(_document, directory);
            }
            else
            {
                Console.WriteLine("No document to save.");
                Console.ReadLine();
            }
        }
    }
}
