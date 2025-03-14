using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Decorators;

public class ItalicDecorator : TextDecorator
{
    public ItalicDecorator(Document doc) : base(doc) { }

    public override void InsertText(string text)
    {
        document.InsertText($"*{text}*");
    }
}