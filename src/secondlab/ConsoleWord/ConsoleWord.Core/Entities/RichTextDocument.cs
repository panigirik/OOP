namespace ConsoleWord.Core.Entities;

public class RichTextDocument : Document
{
    public RichTextDocument(string name, string content = "", string font = "Arial", int textSize = 12)
        : base(name, content, font, textSize) { }
}