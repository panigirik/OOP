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
        private readonly DocumentStorageService _documentStorageService;
        private readonly DocumentEditor _documentEditor;
        private readonly UndoRedoService _undoRedoService;

        public DocumentService(StorageService storageService,
            DocumentFactory documentFactory,
            DocumentStorageService documentStorageService,
            DocumentEditor documentEditor,
            UndoRedoService undoRedoService)
        {
            _documentStorageService = documentStorageService;
            _documentEditor = documentEditor;
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
                    case "Append Text":
                        string originalContent = extension == ".docx"
                            ? _documentEditor.ReadDocxContent(path)
                            : _documentEditor.ReadTextFileContent(path);

                        List<string> contentLines = originalContent.Split('\n').ToList();
                        if (contentLines.Count == 0) contentLines.Add("");

                        int cursorLine = 0;
                        int cursorCol = 0;

                        // Prompt for color selection
                        string currentColor = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Choose a text color:")
                                .AddChoices("red", "green", "yellow", "blue", "white")
                        );

                        bool editing = true;
                        string inputSequence = string.Empty;
                        string clipboard = string.Empty;  // Буфер обмена для копирования/вставки/вырезания
                        (int, int)? selectionStart = null; // Начало выделения

                        while (editing)
                        {
                            AnsiConsole.Clear();
                            AnsiConsole.WriteLine($"--- Interactive Text Editor (ESC = Save, TAB = Exit, Arrows = Move, Typing = Insert) ---");
                            AnsiConsole.WriteLine($"Current color: {currentColor} at Line: {cursorLine + 1}, Col: {cursorCol + 1}");
                            AnsiConsole.WriteLine();

                            // Set the color based on the selected value
                            Console.ForegroundColor = currentColor switch
                            {
                                "red" => ConsoleColor.Red,
                                "green" => ConsoleColor.Green,
                                "yellow" => ConsoleColor.Yellow,
                                "blue" => ConsoleColor.Blue,
                                "white" => ConsoleColor.White,
                                _ => ConsoleColor.White,
                            };

                            // Display the content with the selected color for the entire text
                            for (int i = 0; i < contentLines.Count; i++)
                            {
                                string line = contentLines[i];

                                // Color entire line
                                Console.ForegroundColor = currentColor switch
                                {
                                    "red" => ConsoleColor.Red,
                                    "green" => ConsoleColor.Green,
                                    "yellow" => ConsoleColor.Yellow,
                                    "blue" => ConsoleColor.Blue,
                                    "white" => ConsoleColor.White,
                                    _ => ConsoleColor.White,
                                };

                                if (i == cursorLine)
                                {
                                    if (cursorCol >= line.Length)
                                        line = line.PadRight(cursorCol + 1);

                                    string before = line[..cursorCol];
                                    string cursorChar = line[cursorCol].ToString();
                                    string after = cursorCol + 1 < line.Length ? line[(cursorCol + 1)..] : "";

                                    // Highlight the cursor position
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.Write(before);
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.Write(cursorChar);
                                    Console.ResetColor();
                                    Console.WriteLine(after);
                                }
                                else
                                {
                                    // Display the entire line normally
                                    Console.WriteLine(line);
                                }
                            }

                            var key = Console.ReadKey(true);

                            // Allow exit by pressing Tab instead of "111"
                            if (key.Key == ConsoleKey.Tab)
                            {
                                editing = false;
                                break;
                            }

                            switch (key.Key)
                            {
                                case ConsoleKey.RightArrow when (key.Modifiers & ConsoleModifiers.Shift) != 0:
                                    selectionStart ??= (cursorLine, cursorCol);
                                    if (cursorCol < contentLines[cursorLine].Length)
                                        cursorCol++;
                                    else if (cursorLine < contentLines.Count - 1)
                                    {
                                        cursorLine++;
                                        cursorCol = 0;
                                    }
                                    break;

                                case ConsoleKey.LeftArrow when (key.Modifiers & ConsoleModifiers.Shift) != 0:
                                    selectionStart ??= (cursorLine, cursorCol);
                                    if (cursorCol > 0)
                                        cursorCol--;
                                    else if (cursorLine > 0)
                                    {
                                        cursorLine--;
                                        cursorCol = contentLines[cursorLine].Length;
                                    }
                                    break;

                                case ConsoleKey.UpArrow when (key.Modifiers & ConsoleModifiers.Shift) != 0:
                                    selectionStart ??= (cursorLine, cursorCol);
                                    if (cursorLine > 0)
                                    {
                                        cursorLine--;
                                        cursorCol = Math.Min(cursorCol, contentLines[cursorLine].Length);
                                    }
                                    break;

                                case ConsoleKey.DownArrow when (key.Modifiers & ConsoleModifiers.Shift) != 0:
                                    selectionStart ??= (cursorLine, cursorCol);
                                    if (cursorLine < contentLines.Count - 1)
                                    {
                                        cursorLine++;
                                        cursorCol = Math.Min(cursorCol, contentLines[cursorLine].Length);
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

                                case ConsoleKey.C when (key.Modifiers & ConsoleModifiers.Control) != 0:
                                    if (selectionStart.HasValue)
                                    {
                                        var (startLine, startCol) = selectionStart.Value;
                                        var (endLine, endCol) = (cursorLine, cursorCol);

                                        if (startLine > endLine || (startLine == endLine && startCol > endCol))
                                        {
                                            (startLine, startCol, endLine, endCol) = (endLine, endCol, startLine, startCol);
                                        }


                                        clipboard = string.Join("\n",
                                            contentLines
                                                .Skip(startLine)
                                                .Take(endLine - startLine + 1)
                                                .Select((line, index) =>
                                                {
                                                    if (index == 0 && index == endLine - startLine)
                                                        return line[startCol..endCol];
                                                    if (index == 0)
                                                        return line[startCol..];
                                                    if (index == endLine - startLine)
                                                        return line[..endCol];
                                                    return line;
                                                }));
                                        AnsiConsole.MarkupLine("[green]Copied to clipboard.[/]");
                                    }
                                    break;

                                case ConsoleKey.X when (key.Modifiers & ConsoleModifiers.Control) != 0:
                                    if (selectionStart.HasValue)
                                    {
                                        var (startLine, startCol) = selectionStart.Value;
                                        var (endLine, endCol) = (cursorLine, cursorCol);

                                        if (startLine > endLine || (startLine == endLine && startCol > endCol))
                                        {
                                            (startLine, startCol, endLine, endCol) = (endLine, endCol, startLine, startCol);
                                        }


                                        clipboard = string.Join("\n",
                                            contentLines
                                                .Skip(startLine)
                                                .Take(endLine - startLine + 1)
                                                .Select((line, index) =>
                                                {
                                                    if (index == 0 && index == endLine - startLine)
                                                        return line[startCol..endCol];
                                                    if (index == 0)
                                                        return line[startCol..];
                                                    if (index == endLine - startLine)
                                                        return line[..endCol];
                                                    return line;
                                                }));

                                        // Remove selected text
                                        if (startLine == endLine)
                                        {
                                            var line = contentLines[startLine];
                                            contentLines[startLine] = line[..startCol] + line[endCol..];
                                        }
                                        else
                                        {
                                            var firstLine = contentLines[startLine][..startCol];
                                            var lastLine = contentLines[endLine][endCol..];
                                            contentLines.RemoveRange(startLine, endLine - startLine + 1);
                                            contentLines.Insert(startLine, firstLine + lastLine);
                                        }

                                        cursorLine = startLine;
                                        cursorCol = startCol;
                                        selectionStart = null;

                                        AnsiConsole.MarkupLine("[yellow]Cut to clipboard.[/]");
                                    }
                                    break;

                                case ConsoleKey.V when (key.Modifiers & ConsoleModifiers.Control) != 0:
                                    if (!string.IsNullOrEmpty(clipboard))
                                    {
                                        var lines = clipboard.Split('\n');
                                        var current = contentLines[cursorLine];
                                        var before = current[..cursorCol];
                                        var after = current[cursorCol..];

                                        contentLines[cursorLine] = before + lines[0];
                                        for (int i = 1; i < lines.Length; i++)
                                            contentLines.Insert(cursorLine + i, lines[i]);

                                        if (lines.Length > 1)
                                            contentLines[cursorLine + lines.Length - 1] += after;
                                        else
                                            contentLines[cursorLine] += after;

                                        cursorLine += lines.Length - 1;
                                        cursorCol = lines[^1].Length;

                                        AnsiConsole.MarkupLine("[green]Pasted from clipboard.[/]");
                                    }
                                    break;

                                default:
                                    if (!char.IsControl(key.KeyChar))
                                    {
                                        string line = contentLines[cursorLine];
                                        contentLines[cursorLine] = line.Insert(cursorCol, key.KeyChar.ToString());
                                        cursorCol++;
                                    }
                                    break;
                            }
                        }

                        // После редактирования сохранить изменения
                        string finalText = string.Join('\n', contentLines);

                        try
                        {
                            if (extension == ".docx")
                                _documentEditor.OverwriteDocxFile(path, finalText);
                            else
                                _documentEditor.OverwriteTextFile(path, finalText);

                            AnsiConsole.MarkupLine("[green]Document saved successfully.[/]");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"[red]Failed to save the document: {ex.Message}[/]");
                        }

                        Console.ReadKey();
                        break;

                    case "Delete All Text":
                        _documentEditor.ClearDocxContent(path);
                        AnsiConsole.MarkupLine("[yellow]All content deleted.[/]");
                        Console.ReadKey();
                        break;

                    case "Show Content":
                        Console.Clear();
                        AnsiConsole.WriteLine(content);
                        Console.ReadKey();
                        break;

                    case "Undo":
                        _undoRedoService.Undo();
                        AnsiConsole.MarkupLine("[yellow]Undo completed.[/]");
                        Console.ReadKey();
                        break;

                    case "Redo":
                        _undoRedoService.Redo();
                        AnsiConsole.MarkupLine("[yellow]Redo completed.[/]");
                        Console.ReadKey();
                        break;

                    case "Exit Edit":
                        return;
                }
            }
        }


        
            public void SearchTextInDocument(User currentUser)
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
    
                // Load and display the document content
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

                // Clear console and display content
                Console.Clear();
                AnsiConsole.WriteLine(content);
    
                // Prompt for the search term
                string searchTerm = AnsiConsole.Ask<string>("Enter the text you want to search for:");

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    AnsiConsole.MarkupLine("[red]Please enter a valid search term.[/]");
                    return;
                }

                // Search for the term in the content
                int index = content.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

                if (index == -1)
                {
                    AnsiConsole.MarkupLine($"[yellow]No matches found for '{searchTerm}' in the document.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[green]Found '{searchTerm}' at position {index}.[/]");
                    // Display the text surrounding the search term
                    int start = Math.Max(0, index - 30);
                    int length = Math.Min(60, content.Length - start);
                    string snippet = content.Substring(start, length);

                    AnsiConsole.MarkupLine($"[blue]...{snippet}...[/]");
                }
            }


        
    }
}
