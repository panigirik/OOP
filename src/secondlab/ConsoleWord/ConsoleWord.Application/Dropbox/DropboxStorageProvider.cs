using Dropbox.Api;
using Dropbox.Api.Files;
using Spectre.Console;

namespace ConsoleWord.Application.Dropbox;

public class DropboxStorageProvider : ICloudDropBoxStorageProvider
{
    private readonly string _accessToken;

    public DropboxStorageProvider(string accessToken)
    {
        _accessToken = accessToken;
    }

    public async Task UploadFileAsync(string fileName, byte[] content)
    {
        using var dbx = new DropboxClient(_accessToken);
        using var mem = new MemoryStream(content);

        var uploadPath = "/" + fileName;

        try
        {
            // Выполнение загрузки файла в Dropbox
            var result = await dbx.Files.UploadAsync(
                uploadPath,
                WriteMode.Overwrite.Instance,
                body: mem
            );

            // Красивый вывод с использованием Spectre.Console
            AnsiConsole.MarkupLine($"[green]✅ File uploaded to Dropbox at: {result.PathDisplay}[/]");

            // Использование таблицы для выводом деталей о файле
            var table = new Table();
            table.AddColumn("Property");
            table.AddColumn("Value");

            table.AddRow("Path", result.PathDisplay);
            table.AddRow("Revision", result.Rev);
            table.AddRow("Size", $"{result.Size} bytes");

            AnsiConsole.Render(table);
        }
        catch (DropboxException ex)
        {
            // Вывод ошибки с помощью Spectre.Console
            AnsiConsole.MarkupLine($"[red]❌ Error uploading file: {ex.Message}[/]");
        }
    }
    
    public async Task<byte[]> DownloadFileAsync(string fileName)
    {
        using var dbx = new DropboxClient(_accessToken);
        var downloadPath = "/" + fileName;

        using var response = await dbx.Files.DownloadAsync(downloadPath);
        return await response.GetContentAsByteArrayAsync();
    }

    
}