using ConsoleWord.Application.Interfaces;
using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Services;

public class InsertTextCommand : ICommand
{
    private Document _document;
    private string _text;
    
    public InsertTextCommand(Document doc, string text)
    {
        _document = doc;
        _text = text;
    }

    public void Execute() => _document.InsertText(_text);
    public void Undo() => _document.DeleteText(_document.ToString().Length - _text.Length, _text.Length);
}