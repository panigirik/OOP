namespace ConsoleWord.Core.Entities;

public class MarkdownDocument : Document
{
    public MarkdownDocument(string name, string content = "", string font = "Arial", int textSize = 12)
        : base(name, content, font, textSize)
    {
    }
}