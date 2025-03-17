namespace ConsoleWord.Core.Entities;

public class PlainTextDocument : Document
{
    public PlainTextDocument(string name, string content = "", string font = "Arial", int textSize = 12)
        : base(name, content, font, textSize) { }
}