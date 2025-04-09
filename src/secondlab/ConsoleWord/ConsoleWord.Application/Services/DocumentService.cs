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

            string extension = Path.GetExtension(path).ToLower();

            if (extension != ".docx" && extension != ".txt" && extension != ".json" && extension != ".xml" && extension != ".md")
            {
                AnsiConsole.MarkupLine("[red]Unsupported file format. Supported formats: .docx, .txt, .json, .xml, .md[/]");
                return;
            }

            while (true)
            {
                AnsiConsole.Clear();
                string content;

                try
                {
                    content = extension == ".docx" ? _documentEditor.ReadDocxContent(path) : _documentEditor.ReadTextFileContent(path);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Failed to open document: {ex.Message}[/]");
                    return;
                }

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
                     
                    ////////////////////
                    case "Append Text":
                {
                    string originalContent = extension == ".docx"
                        ? _documentEditor.ReadDocxContent(path)
                        : _documentEditor.ReadTextFileContent(path);

                    List<string> contentLines = originalContent.Split('\n').ToList();
                    if (contentLines.Count == 0) contentLines.Add("");

                    int cursorLine = 0;
                    int cursorCol = 0;
                    string currentColor = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Choose a text color:")
                            .AddChoices("red", "green", "yellow", "blue", "white")
                    );

                    bool editing = true;
                    string inputSequence = string.Empty; // Для хранения введенной последовательности

                    while (editing)
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine($"[bold]--- Interactive Text Editor (ESC = Save, Arrows = Move, Typing = Insert) ---[/]");
                        AnsiConsole.MarkupLine($"Current color: [{currentColor}]{currentColor}[/] at [bold]Line:[/] {cursorLine + 1}, [bold]Col:[/] {cursorCol + 1}");
                        AnsiConsole.WriteLine();

                        for (int i = 0; i < contentLines.Count; i++)
                        {
                            string line = contentLines[i];
                            if (i == cursorLine)
                            {
                                if (cursorCol >= line.Length)
                                    line = line.PadRight(cursorCol + 1);

                                string before = line[..cursorCol];
                                string cursorChar = line[cursorCol].ToString();
                                string after = cursorCol + 1 < line.Length ? line[(cursorCol + 1)..] : "";

                                AnsiConsole.Markup($"[{currentColor}]{before}[/]");
                                AnsiConsole.Markup($"[{currentColor}][invert]{cursorChar}[/][/]");
                                AnsiConsole.MarkupLine($"[{currentColor}]{after}[/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"[{currentColor}]{line}[/]");
                            }
                        }

                        var key = Console.ReadKey(true);

                        // Добавляем текущий символ в последовательность ввода
                        if (!char.IsControl(key.KeyChar))
                        {
                            inputSequence += key.KeyChar.ToString();

                            // Проверяем, не введена ли комбинация "111"
                            if (inputSequence.EndsWith("111"))
                            {
                                editing = false; // Выход из режима редактирования
                                break;
                            }
                        }

                        switch (key.Key)
                        {
                            case ConsoleKey.UpArrow:
                                if (cursorLine > 0) cursorLine--;
                                cursorCol = Math.Min(cursorCol, contentLines[cursorLine].Length);
                                break;

                            case ConsoleKey.DownArrow:
                                if (cursorLine < contentLines.Count - 1) cursorLine++;
                                cursorCol = Math.Min(cursorCol, contentLines[cursorLine].Length);
                                break;

                            case ConsoleKey.LeftArrow:
                                if (cursorCol > 0)
                                    cursorCol--;
                                else if (cursorLine > 0)
                                {
                                    cursorLine--;
                                    cursorCol = contentLines[cursorLine].Length;
                                }
                                break;

                            case ConsoleKey.RightArrow:
                                if (cursorCol < contentLines[cursorLine].Length)
                                    cursorCol++;
                                else if (cursorLine < contentLines.Count - 1)
                                {
                                    cursorLine++;
                                    cursorCol = 0;
                                }
                                break;

                            case ConsoleKey.Backspace:
                                if (cursorCol > 0)
                                {
                                    var line = contentLines[cursorLine];
                                    contentLines[cursorLine] = line.Remove(cursorCol - 1, 1);
                                    cursorCol--;
                                }
                                else if (cursorLine > 0)
                                {
                                    cursorCol = contentLines[cursorLine - 1].Length;
                                    contentLines[cursorLine - 1] += contentLines[cursorLine];
                                    contentLines.RemoveAt(cursorLine);
                                    cursorLine--;
                                }
                                break;

                            case ConsoleKey.Enter:
                                string currentLine = contentLines[cursorLine];
                                string newLine = currentLine[cursorCol..];
                                contentLines[cursorLine] = currentLine[..cursorCol];
                                contentLines.Insert(cursorLine + 1, newLine);
                                cursorLine++;
                                cursorCol = 0;
                                break;

                            case ConsoleKey.Escape:
                                editing = false;
                                break;

                            default:
                                if (!char.IsControl(key.KeyChar))
                                {
                                    var line = contentLines[cursorLine];
                                    line = line.Insert(cursorCol, key.KeyChar.ToString());
                                    contentLines[cursorLine] = line;
                                    cursorCol++;
                                }
                                break;
                        }
                    }

                    // Применим цвет ко всему тексту
                    string finalText = string.Join("\n", contentLines);
                    string coloredText = $"[{currentColor}]{finalText}[/]";

                    var appendCmd = extension == ".docx"
                        ? new AppendTextCommand(path, coloredText, _documentEditor)
                        : new AppendTextCommand(path, coloredText, _documentEditor, isTextFile: true);

                    _undoRedoService.ExecuteCommand(appendCmd);
                    break;
                }

                    
                    //


                    case "Delete All Text":
                        var clearCmd = extension == ".docx"
                            ? new ClearTextCommand(path, _documentEditor)
                            : new ClearTextCommand(path, _documentEditor, isTextFile: true);
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
