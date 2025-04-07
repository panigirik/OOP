using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using ConsoleWord.Infrastracture;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = ConsoleWord.Core.Entities.Document;

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
        _storageService.UploadToCloud(document, format);
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
