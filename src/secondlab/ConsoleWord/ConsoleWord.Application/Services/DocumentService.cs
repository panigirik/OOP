using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ConsoleWord.Infrastracture;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text;
using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.Formatting;
using Spectre.Console;
using Document = ConsoleWord.Core.Entities.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace ConsoleWord.Application.Services
{
    public class DocumentService
    {
        private readonly StorageService _storageService;
        private readonly DocumentFactory _documentFactory;
        private readonly DocumentStorageService _documentStorageService;
        private readonly DocumentEditor _documentEditor;
        private readonly DocumentLoader _documentLoader;
        private readonly InputHelper _inputHelper;

        public DocumentService(StorageService storageService,
            DocumentFactory documentFactory,
            DocumentStorageService documentStorageService,
            DocumentEditor documentEditor,
            DocumentLoader documentLoader,
            InputHelper inputHelper)
        {
            _storageService = storageService;
            _documentFactory = documentFactory;
            _documentStorageService = documentStorageService;
            _documentEditor = documentEditor;
            _documentLoader = documentLoader;
            _inputHelper = inputHelper;
        }

        public void CreateAndSaveDocument()
        {
            string documentName = AnsiConsole.Ask<string>("Enter document name: ");
            string documentText = AnsiConsole.Ask<string>("Enter text: ");
            string font = AnsiConsole.Ask<string>("Choose font (Arial, Times New Roman, Courier New): ");
            int textSize = AnsiConsole.Ask<int>("Choose text size (e.g., 12, 14, 16): ");

            bool isBold = AnsiConsole.Confirm("Make text bold? (y/n)", false);
            bool isItalic = AnsiConsole.Confirm("Make text italic? (y/n)", false);
            bool isUnderline = AnsiConsole.Confirm("Make text underlined? (y/n)", false);

            var document = new Document(documentName, documentText, font, textSize);
            document.IsBold = isBold;
            document.IsItalic = isItalic;
            document.IsUnderline = isUnderline;

            string format = AnsiConsole.Ask<string>("Choose format (docx/xml/json): ").ToLower();
            string storageType = AnsiConsole.Ask<string>("Where do you want to save the document? (local/cloud): ").ToLower();

            string? savedPath = null;

            if (storageType == "local")
            {
                string directory = AnsiConsole.Ask<string>("Enter save directory: ");
                savedPath = _documentStorageService.SaveDocumentLocally(document, directory, format);
            }
            else if (storageType == "cloud")
            {
                _documentStorageService.SaveDocumentToCloud(document, format);
                return;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid storage type.[/]");
                return;
            }

            if (format == "docx" && savedPath != null)
            {
                var formatter = new OpenXmlFormatter();
                formatter.ApplyTextDecorations(savedPath, isBold, isItalic, isUnderline);
            }

            AnsiConsole.MarkupLine("[green]Document created and saved.[/]");
            Console.ReadLine();
        }





        
        private Document LoadMarkdown(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0)
                throw new InvalidOperationException("Empty markdown file");

            string name = Path.GetFileNameWithoutExtension(filePath);
            var content = new StringBuilder();

            for (int i = 1; i < lines.Length; i++) // пропускаем заголовок
            {
                content.AppendLine(lines[i]);
            }

            return new Document(name, content.ToString(), "Arial", 12);
        }



        

        
        public void OpenAndEditDocument(User currentUser)
        {
            string path = AnsiConsole.Ask<string>("Enter full file path to open:");

            if (!File.Exists(path))
            {
                AnsiConsole.MarkupLine("[red]File not found.[/]");
                return;
            }

            while (true)
            {
                AnsiConsole.Clear();
                string content = _documentEditor.ReadDocxContent(path);
                AnsiConsole.MarkupLine("[bold]--- Document Content ---[/]");
                AnsiConsole.WriteLine(content);
                AnsiConsole.MarkupLine("[bold]------------------------[/]");

                List<string> actions = new() { "Show Content", "Exit Edit" };

                if (currentUser.Role.HasPermission("Edit"))
                {
                    actions.Insert(0, "Append Text");
                    actions.Insert(1, "Delete All Text");
                }

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an action:")
                        .AddChoices(actions)
                );

                switch (choice)
                {
                    case "Append Text":
                        string toAppend = AnsiConsole.Ask<string>("Enter text to append:");
                        _documentEditor.AppendTextToDocx(path, toAppend);
                        break;

                    case "Delete All Text":
                        _documentEditor.ClearDocxContent(path);
                        break;

                    case "Show Content":
                        break;

                    case "Exit Edit":
                        return;
                }
            }
        }

        
        public void EditDocument(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File does not exist.");
                return;
            }

            while (true)
            {
                Console.WriteLine("Choose an action: [1] Append Text [2] Delete All Text [3] Show Content [4] Exit Edit");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter text to append: ");
                        string newText = Console.ReadLine();
                        _documentEditor.AppendTextToDocx(path, newText);
                        break;
                    case "2":
                        _documentEditor.ClearDocxContent(path);
                        break;
                    case "3":
                        Console.WriteLine(_documentEditor.ReadDocxContent(path));
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        



        
        public Document LoadDocument()
        {
            string filePath = _inputHelper.GetUserInput("Enter full file path to open: ");
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist.");
                return null;
            }

            string extension = Path.GetExtension(filePath).ToLower();

            try
            {
                return extension switch
                {
                    ".docx" => _documentLoader.LoadDocx(filePath),
                    ".xml" => _documentLoader.LoadXml(filePath),
                    ".json" => _documentLoader.LoadJson(filePath),
                    _ => throw new InvalidOperationException("Unsupported file format")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load document: {ex.Message}");
                return null;
            }
        }




    }
}
