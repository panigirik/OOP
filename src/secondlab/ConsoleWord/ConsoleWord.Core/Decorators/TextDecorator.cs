using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Decorators;

public abstract class TextDecorator : Document
{
    protected Document document;

    public TextDecorator(Document doc) : base(doc.Name)
    {
        this.document = doc;
    }
}