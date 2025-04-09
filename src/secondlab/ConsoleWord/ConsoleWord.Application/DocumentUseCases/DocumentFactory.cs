using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using ConsoleWord.Infrastracture;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Spectre.Console;
using Document = ConsoleWord.Core.Entities.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace ConsoleWord.Application.DocumentUseCases;

public class DocumentFactory
{
    
    private readonly StorageService _storageService;

    public DocumentFactory(StorageService storageService)
    {
        _storageService = storageService;
    }
    
    public Document CreateDocument(string documentName, string documentText, string font, int textSize, bool isBold, bool isItalic, bool isUnderline)
    {
        var document = new Document(documentName, documentText, font, textSize)
        {
            IsBold = isBold,
            IsItalic = isItalic,
            IsUnderline = isUnderline
        };

        return document;
    }
    
    public static Document LoadFromStream(Stream stream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        stream.Position = 0;

        return extension switch
        {
            ".docx" => LoadDocx(stream),
            ".json" => LoadJson(stream),
            ".xml" => LoadXml(stream),
            ".md"   => LoadMarkdown(stream),
            _ => throw new NotSupportedException("Unsupported file format.")
        };
    }

    private static Document LoadDocx(Stream stream)
    {
        using var wordDoc = WordprocessingDocument.Open(stream, false); // false = read-only
        var body = wordDoc.MainDocumentPart.Document.Body;
        string text = body.InnerText;

        return new Document
        {
            Name = Guid.NewGuid().ToString(),
            Content = new StringBuilder(text),
            Font = "Times New Roman", // можно подставить значения по умолчанию
            TextSize = 12,            // по умолчанию, если точные значения не извлекаются
            IsBold = false,
            IsItalic = false,
            IsUnderline = false
        };
    }


    private static Document LoadJson(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<Document>(json);
    }

    private static Document LoadXml(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(Document));
        return (Document)serializer.Deserialize(stream);
    }

    private static Document LoadMarkdown(Stream stream)
    {
        using var reader = new StreamReader(stream);
        string content = reader.ReadToEnd();
    
        return new Document
        {
            Name = "ImportedFromMarkdown",
            Content = new StringBuilder(content) 
        };
    }

    
    public string SaveDocument(Document document, string directory, string format)
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
            return filePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving document: {ex.Message}");
            return null;
        }
    }

    public static void SaveAsDocx(Document document, Stream outputStream)
    {
        try
        {
            using var wordDocument = WordprocessingDocument.Create(outputStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true);
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

            paragraph.Append(run);
            body.Append(paragraph);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error while saving document as docx: {ex.Message}");
            throw;
        }
    }


    
    public void SaveAsDocx(Document document, string filePath)
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

    public static void SaveAsJson(Document document, Stream stream)
    {
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        JsonSerializer.Serialize(writer, document);
    }

    public static void SaveAsXml(Document document, Stream stream)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Document));
        serializer.Serialize(stream, document);
    }

    public static void SaveAsMarkdown(Document document, Stream stream)
    {
        using var writer = new StreamWriter(stream);
        writer.WriteLine($"# {document.Name}");
        writer.WriteLine();

        string content = document.Content.ToString();
        if (document.IsBold) content = $"**{content}**";
        if (document.IsItalic) content = $"*{content}*";
        if (document.IsUnderline) content = $"<u>{content}</u>";

        writer.WriteLine(content);
    }

    
    public void SaveAsXml(Document document, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Document));
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, document);
        }

        Console.WriteLine($"Document saved as XML successfully: {filePath}");
    }

    public void SaveDocumentToCloud(Document document, string format)
    {
        _storageService.UploadToCloudAsync(document, format);
        Console.WriteLine("Document uploaded to cloud (stub).");
    }

    
    public void SaveAsJson(Document document, string filePath)
    {
        string json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public void SaveAsMarkdown(Document document, string filePath)
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

}
