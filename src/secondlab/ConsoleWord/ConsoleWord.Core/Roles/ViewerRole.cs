using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Roles;

public class ViewerRole : UserRole
{
    public ViewerRole() : base("Viewer")
    {
        Permissions.Add("Read");
    }

    public override void AccessDocument(Document doc)
    {
        Console.WriteLine($"Viewer is reading the document: {doc.Name}");
    }
}