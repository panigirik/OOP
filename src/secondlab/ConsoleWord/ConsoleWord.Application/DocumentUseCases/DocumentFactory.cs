using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using ConsoleWord.Application.Services;
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
        using var wordDoc = WordprocessingDocument.Open(stream, false); 
        var body = wordDoc.MainDocumentPart.Document.Body;
        string text = body.InnerText;

        return new Document
        {
            Name = Guid.NewGuid().ToString(),
            Content = new StringBuilder(text),
            Font = "Times New Roman",
            TextSize = 12,           
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

    public void SaveAsTxt(Document document, string filePath)
    {
        string content = document.Content.ToString();
        File.WriteAllText(filePath, content);
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
    
    
    public void SaveAsJson(Document document, string filePath)
    {
        string json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public void SaveAsMarkdown(Document document, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {document.Name}");
        sb.AppendLine();
        
        string content = document.Content.ToString();
        
        if (document.IsBold) content = $"**{content}**";
        if (document.IsItalic) content = $"*{content}*";
        if (document.IsUnderline) content = $"<u>{content}</u>"; 

        sb.AppendLine(content);

        File.WriteAllText(filePath, sb.ToString());
    }

}
