using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Decorators;

public class UnderlineDecorator : TextDecorator
{
    public UnderlineDecorator(Document doc) : base(doc) { }

    public override void InsertText(string text)
    {
        document.InsertText($"__{text}__");
    }
}