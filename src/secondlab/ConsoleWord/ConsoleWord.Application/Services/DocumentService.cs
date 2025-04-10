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

                        case ConsoleKey.C when (key.Modifiers & ConsoleModifiers.Control) != 0:
                            // Ctrl+C (копирование)
                            clipboard = GetSelectedText(contentLines, selectionStart, cursorLine, cursorCol);
                            break;

                        case ConsoleKey.X when (key.Modifiers & ConsoleModifiers.Control) != 0:
                            // Ctrl+X (вырезание)
                            clipboard = GetSelectedText(contentLines, selectionStart, cursorLine, cursorCol);
                            RemoveSelectedText(ref contentLines, selectionStart, cursorLine, cursorCol, out clipboard);
                            break;


                        case ConsoleKey.V when (key.Modifiers & ConsoleModifiers.Control) != 0:
                            // Ctrl+V (вставка)
                            InsertTextFromClipboard(ref contentLines, clipboard, ref cursorLine, ref cursorCol);
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

                string finalText = string.Join("\n", contentLines);

                // Append text to the file or document with color reset
                var appendCmd = extension == ".docx"
                    ? new AppendTextCommand(path, finalText, _documentEditor)
                    : new AppendTextCommand(path, finalText, _documentEditor, isTextFile: true);

                _undoRedoService.ExecuteCommand(appendCmd);
                break;

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


        




static string GetSelectedText(List<string> contentLines, (int, int)? selectionStart, int cursorLine, int cursorCol)
{
    if (!selectionStart.HasValue)
        return string.Empty;

    var (startLine, startCol) = selectionStart.Value;
    string selectedText = string.Empty;

    if (startLine == cursorLine)
    {
        selectedText = contentLines[startLine].Substring(startCol, cursorCol - startCol);
    }
    else
    {
        // Multi-line selection can be implemented
    }

    return selectedText;
}

static void InsertTextFromClipboard(ref List<string> contentLines, string clipboard, ref int cursorLine, ref int cursorCol)
{
    if (string.IsNullOrEmpty(clipboard))
        return;

    var line = contentLines[cursorLine];
    contentLines[cursorLine] = line.Insert(cursorCol, clipboard);
    cursorCol += clipboard.Length;
}

    public static void RemoveSelectedText(ref List<string> contentLines, (int, int)? selectionStart, int cursorLine, int cursorCol, out string clipboardText)
    {
        clipboardText = string.Empty;

        if (!selectionStart.HasValue)
            return;

        var (startLine, startCol) = selectionStart.Value;

        // Если выделение на одной строке
        if (startLine == cursorLine)
        {
            // Сохраняем выделенный текст в буфер (в память)
            clipboardText = contentLines[startLine].Substring(startCol, cursorCol - startCol);

            // Удаляем выделенный текст
            contentLines[startLine] = contentLines[startLine].Remove(startCol, cursorCol - startCol);
        }
        else
        {
            // Сохраняем текст между началом и концом выделения в буфер
            string selectedText = string.Empty;

            // Сначала часть на первой строке
            selectedText += contentLines[startLine].Substring(startCol);

            // Затем все строки между первой и последней (если есть)
            for (int i = startLine + 1; i < cursorLine; i++)
            {
                selectedText += contentLines[i] + Environment.NewLine; // Добавляем строки между выделением
            }

            // Наконец, часть на последней строке
            selectedText += contentLines[cursorLine].Substring(0, cursorCol);

            // Сохраняем весь текст в "буфер"
            clipboardText = selectedText;

            // Удаляем выделенный текст
            // Удаление текста на первой строке
            contentLines[startLine] = contentLines[startLine].Remove(startCol);

            // Удаление текста во всех строках между startLine и cursorLine
            for (int i = startLine + 1; i < cursorLine; i++)
            {
                contentLines[i] = string.Empty; // Очищаем строки
            }

            // Удаление текста на последней строке
            contentLines[cursorLine] = contentLines[cursorLine].Substring(cursorCol);
        }
    }
        
    }
}
