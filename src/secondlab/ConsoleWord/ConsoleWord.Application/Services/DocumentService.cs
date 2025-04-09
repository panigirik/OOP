using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ConsoleWord.Infrastracture;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text;
using ConsoleWord.Application.Commands;
using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.Formatting;
using Spectre.Console;
using Document = ConsoleWord.Core.Entities.Document;


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
        private readonly UndoRedoService _undoRedoService;

        public DocumentService(StorageService storageService,
            DocumentFactory documentFactory,
            DocumentStorageService documentStorageService,
            DocumentEditor documentEditor,
            DocumentLoader documentLoader,
            InputHelper inputHelper,
            UndoRedoService undoRedoService)
        {
            _storageService = storageService;
            _documentFactory = documentFactory;
            _documentStorageService = documentStorageService;
            _documentEditor = documentEditor;
            _documentLoader = documentLoader;
            _inputHelper = inputHelper;
            _undoRedoService = undoRedoService;
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

            var document = new Document(documentName, documentText, font, textSize)
            {
                IsBold = isBold,
                IsItalic = isItalic,
                IsUnderline = isUnderline
            };

            var format = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose format:")
                    .AddChoices("docx", "xml", "json", "md")
            );

            var storageType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Where do you want to save the document?")
                    .AddChoices("local", "cloud")
            );

            if (storageType == "local")
            {
                string directory = AnsiConsole.Ask<string>("Enter save directory: ");
                var createCommand = new CreateDocumentCommand(document, directory, format, _documentStorageService);
                _undoRedoService.ExecuteCommand(createCommand);

                if (format == "docx")
                {
                    var savedPath = Path.Combine(directory, $"{documentName}.docx");
                    var formatter = new OpenXmlFormatter();
                    formatter.ApplyTextDecorations(savedPath, isBold, isItalic, isUnderline);
                }
            }
            else if (storageType == "cloud")
            {
                _documentStorageService.SaveDocumentToCloud(document, format);
            }

            AnsiConsole.MarkupLine("[green]Document created and saved.[/]");
            Console.ReadLine();
        }


        public void DeleteDocumentByPath()
        {
            string path = AnsiConsole.Ask<string>("Enter full file path to delete:");

            if (!File.Exists(path))
            {
                AnsiConsole.MarkupLine("[red]File not found. Nothing to delete.[/]");
                return;
            }

            var deleteCommand = new DeleteDocumentCommand(path);
            _undoRedoService.ExecuteCommand(deleteCommand);

            AnsiConsole.MarkupLine("[green]Document successfully deleted.[/]");
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

                List<string> actions = new() { "Show Content", "Undo", "Redo", "Exit Edit" };

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
                        var appendCmd = new AppendTextCommand(path, toAppend, _documentEditor);
                        _undoRedoService.ExecuteCommand(appendCmd);
                        break;

                    case "Delete All Text":
                        var clearCmd = new ClearTextCommand(path, _documentEditor);
                        _undoRedoService.ExecuteCommand(clearCmd);
                        break;

                    case "Undo":
                        _undoRedoService.Undo();
                        break;

                    case "Redo":
                        _undoRedoService.Redo();
                        break;

                    case "Show Content":
                        break;

                    case "Exit Edit":
                        return;
                }
            }
        }


    }
}
