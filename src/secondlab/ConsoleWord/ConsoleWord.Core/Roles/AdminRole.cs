using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Roles;

public class AdminRole : UserRole
{
    public AdminRole() : base("Admin")
    {
        Permissions.AddRange(new[] { "Read", "Edit", "Delete", "ManageUsers" });
    }

    public override void AccessDocument(Document doc)
    {
        Console.WriteLine($"Admin is managing the document: {doc.Name}");
    }
}
