using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Commands;

public class CreateDocumentCommand : IDocumentCommand
{
    private readonly Document _document;
    private readonly string _directory;
    private readonly string _format;
    private readonly DocumentStorageService _documentStorageService;
    private string? _savedPath;

    public CreateDocumentCommand(Document document, string directory, string format, DocumentStorageService documentStorageService)
    {
        _document = document;
        _directory = directory;
        _format = format.ToLower();
        _documentStorageService = documentStorageService;
    }

    public void Execute()
    {
        _savedPath = _documentStorageService.SaveDocumentLocally(_document, _directory, _format);
    }

    public void Undo()
    {
        if (string.IsNullOrEmpty(_savedPath))
        {
            Console.WriteLine("Nothing to undo. No path available.");
            return;
        }

        try
        {
            if (File.Exists(_savedPath))
            {
                File.Delete(_savedPath);
                Console.WriteLine($"[Undo] Document deleted: {_savedPath}");
            }
            else
            {
                Console.WriteLine($"[Undo] File not found: {_savedPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Undo] Error while deleting file: {ex.Message}");
        }
    }
}