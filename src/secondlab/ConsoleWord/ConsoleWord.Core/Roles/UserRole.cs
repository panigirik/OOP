using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Roles;

    public class UserRole
    {
        public string RoleName { get; set; }
        
        public List<string> Permissions { get; set; }

        public UserRole(string roleName)
        {
            RoleName = roleName;
            Permissions = new List<string>();
        }
        
        public virtual void AccessDocument(Document doc)
        {
            Console.WriteLine($"{RoleName} is accessing the document: {doc.Name}");
        }
        
        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }
    }
