using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Roles;

public class AdminRole : IUserRole
{
    public void AccessDocument(Document doc)
    {
        Console.WriteLine($"Admin is managing the document: {doc.Name}");
    }
}
