using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Roles;

public class EditorRole : UserRole
{
    public EditorRole() : base("Editor")
    {
        Permissions.AddRange(new[] { "Read", "Edit" });
    }

    public override void AccessDocument(Document doc)
    {
        Console.WriteLine($"Editor is editing the document: {doc.Name}");
    }
}
