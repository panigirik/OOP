using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture;

namespace ConsoleWord.Application.Services;

public class DocumentService
{
    private readonly StorageService _storageService;
    private readonly FormattingService _formattingService;

    public DocumentService(StorageService storageService, FormattingService formattingService)
    {
        _storageService = storageService;
        _formattingService = formattingService;
    }

    public void SaveDocumentLocally(Document doc, string path)
    {
        _storageService.SaveToLocal(doc, path);
    }

    public void UploadDocumentToCloud(Document doc, string cloudPath)
    {
        _storageService.UploadToCloud(doc, cloudPath);
    }

    public Document LoadDocumentFromLocal(string path)
    {
        return _storageService.LoadFromLocal(path);
    }

    public Document DownloadDocumentFromCloud(string cloudPath)
    {
        return _storageService.DownloadFromCloud(cloudPath);
    }

    public Document FormatBold(Document doc)
    {
        return _formattingService.ApplyBold(doc);
    }

    public Document FormatItalic(Document doc)
    {
        return _formattingService.ApplyItalic(doc);
    }

    public Document FormatUnderline(Document doc)
    {
        return _formattingService.ApplyUnderline(doc);
    }
}
