using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Roles;

public class ViewerRole : UserRole
{
    public ViewerRole(List<string> permissions) : base("Viewer")
    {
        Permissions.AddRange(permissions);
    }
}