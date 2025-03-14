using System.Text;

namespace ConsoleWord.Core.Entities;

public abstract class Document
{
    public string Name { get; set; }
    public StringBuilder Content { get; set; } = new StringBuilder();

    public Document(string name)
    {
        Name = name;
    }

    public virtual void InsertText(string text) => Content.Append(text);
    public virtual void DeleteText(int startIndex, int length) => Content.Remove(startIndex, length);
    public override string ToString() => Content.ToString();
}