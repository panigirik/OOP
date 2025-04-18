using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Core.Entities;
using Spectre.Console;

namespace ConsoleWord.Application.Helpers;

public class SaveDocumentToCloudHelper
{
    private readonly DocumentEditor _documentEditor;
    private readonly DocumentStorageService _documentStorageService;

    public SaveDocumentToCloudHelper(DocumentEditor documentEditor,
        DocumentStorageService documentStorageService)
    {
        _documentEditor = documentEditor;
        _documentStorageService = documentStorageService;
    }

    public void SaveDocumentToCloud()
    {
        var filePath = AnsiConsole.Ask<string>("Enter the [green]path[/] to the .docx file you want to upload:");

        if (!File.Exists(filePath))
        {
            AnsiConsole.MarkupLine("[red]File not found.[/]");
            return;
        }
            
        var format = AnsiConsole.Ask<string>("Enter the [green]format[/] for the document (e.g., 'docx'):");
            
        Document document = _documentEditor.LoadDocument(filePath); 

        _documentStorageService.SaveDocumentToCloud(document, format);

        AnsiConsole.MarkupLine("[green]Document saved to cloud successfully.[/]");
    }
}