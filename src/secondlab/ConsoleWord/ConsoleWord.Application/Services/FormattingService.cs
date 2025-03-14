using ConsoleWord.Core.Decorators;
using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Services;

public class FormattingService
{
    public Document ApplyBold(Document doc)
    {
        return new BoldDecorator(doc);
    }

    public Document ApplyItalic(Document doc)
    {
        return new ItalicDecorator(doc);
    }

    public Document ApplyUnderline(Document doc)
    {
        return new UnderlineDecorator(doc);
    }
}
