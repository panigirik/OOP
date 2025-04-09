using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Interfaces;

namespace ConsoleWord.Application.Commands;

public class AppendTextCommand : IDocumentCommand
{
    private readonly string _path;
    private readonly string _text;
    private readonly DocumentEditor _editor;

    public AppendTextCommand(string path, string text, DocumentEditor editor)
    {
        _path = path;
        _text = text;
        _editor = editor;
    }

    public void Execute()
    {
        _editor.AppendTextToDocx(_path, _text);
    }

    public void Undo()
    {
        _editor.RemoveLastAppendedText(_path, _text.Length);
    }
}

