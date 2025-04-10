using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Dropbox;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using Spectre.Console;

namespace ConsoleWord.Application.Services;

public class StorageService
{
    private readonly IStorageProvider _localStorage;
    private readonly ICloudDropBoxStorageProvider _cloudStorage;

    public StorageService(IStorageProvider localStorage, ICloudDropBoxStorageProvider cloudStorage)
    {
        _localStorage = localStorage;
        _cloudStorage = cloudStorage;
    }

    public void SaveToLocal(Document doc, string path)
    {
        _localStorage.Save(doc, path);
    }

    public Document LoadFromLocal(string path)
    {
        return _localStorage.Load(path);
    }

    public async Task<Document> DownloadFromCloudAsync(string cloudPath)
    {
        byte[] fileBytes = await _cloudStorage.DownloadFileAsync(cloudPath);

        using var stream = new MemoryStream(fileBytes);
        return DocumentFactory.LoadFromStream(stream, cloudPath); // метод, распознающий формат по расширению
    }

    public async Task UploadToCloudAsync(Document document, string format)
    {
        string fileName = document.Name + "." + format;
        byte[] fileContent;

        using (var stream = new MemoryStream())
        {
            switch (format)
            {
                case "docx":
                    DocumentFactory.SaveAsDocx(document, stream); break;
                case "json":
                    DocumentFactory.SaveAsJson(document, stream); break;
                case "xml":
                    DocumentFactory.SaveAsXml(document, stream); break;
                case "md":
                    DocumentFactory.SaveAsMarkdown(document, stream); break;
                default:
                    AnsiConsole.WriteLine("Unsupported format");
                    return;
            }

            if (stream.Length == 0)
            {
                AnsiConsole.WriteLine("[red]Error: The document was not saved correctly to the stream.[/]");
                return;
            }
            
            fileContent = stream.ToArray();
        }

        await _cloudStorage.UploadFileAsync(fileName, fileContent);
    }
}
