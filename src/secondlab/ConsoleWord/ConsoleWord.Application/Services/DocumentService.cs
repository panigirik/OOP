using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ConsoleWord.Infrastracture;
using ConsoleWord.Core.Decorators;
using System.Xml.Serialization;
using System.Text.Json;
using System.IO;
using System.Text;
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

        public DocumentService(StorageService storageService)
        {
            _storageService = storageService;
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
                savedPath = SaveDocumentLocally(document, directory, format);
            }
            else if (storageType == "cloud")
            {
                SaveDocumentToCloud(document, format);
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

        public string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();
            }
            return input;
        }

        public bool AskYesNo(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim().ToLower();

            while (input != "y" && input != "n")
            {
                Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim().ToLower();
            }

            return input == "y";
        }

        private int GetValidatedInteger(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                Console.WriteLine("Invalid number. Please enter a positive integer.");
                Console.Write(prompt);
            }
            return value;
        }

        public string GetValidDirectory(string prompt)
        {
            Console.Write(prompt);
            string directory = Console.ReadLine()?.Trim();
            while (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                Console.WriteLine("Invalid directory. Please enter a valid path.");
                Console.Write(prompt);
                directory = Console.ReadLine()?.Trim();
            }
            return directory;
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

        
        private void SaveAsMarkdown(Document document, string filePath)
        {
            var sb = new StringBuilder();

            // Пример простой разметки markdown: жирный, курсив, подчеркивание
            sb.AppendLine($"# {document.Name}");
            sb.AppendLine();

            // Можно применить базовые markdown-стили (как опцию — расширяемо)
            string content = document.Content.ToString();

            // Обработка базовых стилей (например, жирный шрифт как **text**)
            if (document.IsBold) content = $"**{content}**";
            if (document.IsItalic) content = $"*{content}*";
            if (document.IsUnderline) content = $"<u>{content}</u>"; // Markdown не поддерживает underline напрямую

            sb.AppendLine(content);

            File.WriteAllText(filePath, sb.ToString());
        }


        public string SaveDocumentLocally(Document document, string directory, string format)
        {
            string filePath = Path.Combine(directory, document.Name + $".{format}");
            try
            {
                switch (format)
                {
                    case "docx":
                        SaveAsDocx(document, filePath);
                        break;
                    case "xml":
                        SaveAsXml(document, filePath);
                        break;
                    case "json":
                        SaveAsJson(document, filePath);
                        break;
                    case "md":
                        SaveAsMarkdown(document, filePath);
                        break;
                    default:
                        Console.WriteLine("Invalid format selected.");
                        return null;
                }
                Console.WriteLine($"Document saved successfully: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving document: {ex.Message}");
                return null;
            }
        }


        private void SaveAsDocx(Document document, string filePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(new Body());
                Body body = mainPart.Document.Body;

                Paragraph paragraph = new Paragraph();
                Run run = new Run(new Text(document.Content.ToString()));
                RunProperties runProperties = new RunProperties
                {
                    FontSize = new FontSize() { Val = (document.TextSize * 2).ToString() },
                    RunFonts = new RunFonts() { Ascii = document.Font }
                };
                run.PrependChild(runProperties);

                // These will be overridden later with ApplyTextDecorations
                paragraph.Append(run);
                body.Append(paragraph);
            }
        }

        private void SaveAsXml(Document document, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Document));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, document);
            }

            Console.WriteLine($"Document saved as XML successfully: {filePath}");
        }

        private void SaveAsJson(Document document, string filePath)
        {
            string json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public void SaveDocumentToCloud(Document document, string format)
        {
            _storageService.UploadToCloud(document, format);
            Console.WriteLine("Document uploaded to cloud (stub).");
        }




        public string ReadDocxContent(string path)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, false))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;
                return body.InnerText;
            }
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
                string content = ReadDocxContent(path);
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
                        AppendTextToDocx(path, toAppend);
                        break;

                    case "Delete All Text":
                        ClearDocxContent(path);
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
                        AppendTextToDocx(path, newText);
                        break;
                    case "2":
                        ClearDocxContent(path);
                        break;
                    case "3":
                        Console.WriteLine(ReadDocxContent(path));
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        
        public void AppendTextToDocx(string path, string textToAppend)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;

                Paragraph paragraph = new Paragraph();
                Run run = new Run();
                run.AppendChild(new Text(textToAppend));
                paragraph.Append(run);

                body.Append(paragraph);
                wordDoc.MainDocumentPart.Document.Save();
            }
        }


        public void ClearDocxContent(string path)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;
                body.RemoveAllChildren();
                wordDoc.MainDocumentPart.Document.Save();
            }
        }

        
        public Document LoadDocument()
        {
            string filePath = GetUserInput("Enter full file path to open: ");
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
                    ".docx" => LoadDocx(filePath),
                    ".xml" => LoadXml(filePath),
                    ".json" => LoadJson(filePath),
                    _ => throw new InvalidOperationException("Unsupported file format")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load document: {ex.Message}");
                return null;
            }
        }

        private Document LoadDocx(string filePath)
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart.Document.Body;

            var text = new StringBuilder();
            foreach (var paragraph in body.Elements<Paragraph>())
            {
                foreach (var run in paragraph.Elements<Run>())
                {
                    text.Append(run.InnerText);
                }
                text.AppendLine();
            }

            return new Document(Path.GetFileNameWithoutExtension(filePath), text.ToString(), "Arial", 12);
        }

        private Document LoadXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Document));
            using var reader = new StreamReader(filePath);
            return (Document)serializer.Deserialize(reader);
        }

        private Document LoadJson(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Document>(json);
        }
    }
}
