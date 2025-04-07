using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture;

namespace ConsoleWord.Application.DocumentUseCases;

public class DocumentStorageService
{
    private readonly StorageService _storageService;
    private readonly DocumentFactory _documentFactory;

    public DocumentStorageService(StorageService storageService,
        DocumentFactory documentFactory)
    {
        _storageService = storageService;
        _documentFactory = documentFactory;
    }

    public void SaveDocumentToCloud(Document document, string format)
    {
        _storageService.UploadToCloud(document, format);
        Console.WriteLine("Document uploaded to cloud.");
    }

    public string SaveDocumentLocally(Document document, string directory, string format)
    {
        string filePath = Path.Combine(directory, document.Name + $".{format}");
        try
        {
            switch (format)
            {
                case "docx":
                    _documentFactory.SaveAsDocx(document, filePath);
                    break;
                case "xml":
                    _documentFactory.SaveAsXml(document, filePath);
                    break;
                case "json":
                    _documentFactory.SaveAsJson(document, filePath);
                    break;
                case "md":
                    _documentFactory.SaveAsMarkdown(document, filePath);
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
}
