using ConsoleWord.Core.Entities;

namespace ConsoleWord.Core.Roles;

    public class UserRole
    {
        // Имя роли (Admin, Editor, Viewer и т. д.)
        public string RoleName { get; set; }

        // Права роли (например, разрешение на редактирование, чтение и т. д.)
        public List<string> Permissions { get; set; }

        public UserRole(string roleName)
        {
            RoleName = roleName;
            Permissions = new List<string>();
        }

        // Метод для доступа к документу (будет переопределяться в наследуемых классах)
        public virtual void AccessDocument(Document doc)
        {
            // Базовая реализация может быть общей или пустой,
            // в зависимости от вашей логики
            Console.WriteLine($"{RoleName} is accessing the document: {doc.Name}");
        }

        // Метод для проверки, есть ли у роли разрешение на выполнение действия
        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }
    }
