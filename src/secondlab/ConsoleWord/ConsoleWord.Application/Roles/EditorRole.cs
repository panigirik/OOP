using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Roles;

public class EditorRole : IUserRole
{
    public void AccessDocument(Document doc)
    {
        Console.WriteLine($"Editor is editing the document: {doc.Name}");
    }
}
