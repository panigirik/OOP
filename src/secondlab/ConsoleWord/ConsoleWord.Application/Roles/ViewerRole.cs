using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Roles;

public class ViewerRole : IUserRole
{
    public void AccessDocument(Document doc)
    {
        Console.WriteLine($"Viewer is reading the document: {doc.Name}");
    }
}
