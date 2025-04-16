using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Roles;

public class EditorRole : UserRole
{
    public EditorRole(List<string> permissions) : base("Editor")
    {
        Permissions.AddRange(permissions);
    }
}