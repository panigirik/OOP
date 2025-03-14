using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Decorators;

public class BoldDecorator : TextDecorator
{
    public BoldDecorator(Document doc) : base(doc) { }

    public override void InsertText(string text)
    {
        document.InsertText($"**{text}**");
    }
}