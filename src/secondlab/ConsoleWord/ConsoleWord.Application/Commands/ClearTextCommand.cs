using ConsoleWord.Application.Commands;
using ConsoleWord.Application.DocumentUseCases;

public class ClearTextCommand : IDocumentCommand
{
    private readonly string _path;
    private readonly DocumentEditor _editor;
    private readonly bool _isTextFile;
    private string? _backupContent;

    public ClearTextCommand(string path, DocumentEditor editor, bool isTextFile = false)
    {
        _path = path;
        _editor = editor;
        _isTextFile = isTextFile;
    }

    public void Execute()
    {
        _backupContent = _isTextFile
            ? _editor.ReadTextFileContent(_path)
            : _editor.ReadDocxContent(_path);

        if (_isTextFile)
            _editor.OverwriteTextFile(_path, string.Empty);
        else
            _editor.ClearDocxContent(_path);
    }

    public void Undo()
    {
        if (_backupContent is not null)
        {
            if (_isTextFile)
                _editor.OverwriteTextFile(_path, _backupContent);
            else
                _editor.AppendTextToDocx(_path, _backupContent);
        }
    }
}