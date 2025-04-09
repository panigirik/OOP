using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Interfaces;

namespace ConsoleWord.Application.Commands;

public class ClearTextCommand : IDocumentCommand
{
    private readonly string _path;
    private readonly DocumentEditor _editor;
    private string? _backupContent;

    public ClearTextCommand(string path, DocumentEditor editor)
    {
        _path = path;
        _editor = editor;
    }

    public void Execute()
    {
        _backupContent = _editor.ReadDocxContent(_path);
        _editor.ClearDocxContent(_path);
    }

    public void Undo()
    {
        if (_backupContent is not null)
        {
            _editor.AppendTextToDocx(_path, _backupContent);
        }
    }
}
