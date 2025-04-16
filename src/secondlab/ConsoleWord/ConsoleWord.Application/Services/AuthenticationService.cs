using ConsoleWord.Core.Entities;
using ConsoleWord.Core.Roles;
using ConsoleWord.Application.Helpers;

namespace ConsoleWord.Application.Services
{
    public class AuthenticationService
    {
        private const string UsersFilePath = "users.txt";
        private Dictionary<string, User> _users;

        public AuthenticationService()
        {
            _users = new Dictionary<string, User>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (!File.Exists(UsersFilePath))
            {
                Console.WriteLine("User data file not found!");
                return;
            }

            var permissionManager = new PermissionManager();

            foreach (var line in File.ReadAllLines(UsersFilePath))
            {
                var parts = line.Split(':');
                if (parts.Length == 3)
                {
                    string username = parts[0];
                    string password = parts[1];
                    string role = parts[2];

                    UserRole userRole = role switch
                    {
                        "Admin" => new AdminRole(),
                        "Editor" => new EditorRole(permissionManager.GetPermissionsForRole("Editor")),
                        "Viewer" => new ViewerRole(permissionManager.GetPermissionsForRole("Viewer")),
                        _ => throw new Exception("Unknown role!")
                    };

                    _users.Add(username, new User(username, password, userRole));
                }
            }
        }

        public User Authenticate(string username, string password)
        {
            if (_users.ContainsKey(username) && _users[username].Password == password)
            {
                return _users[username];
            }
            return null;
        }

        public void Register(string username, string password, string role)
        {
            if (_users.ContainsKey(username))
            {
                Console.WriteLine("User already exists!");
                return;
            }

            var permissionManager = new PermissionManager();
            UserRole userRole = role switch
            {
                "Admin" => new AdminRole(),
                "Editor" => new EditorRole(permissionManager.GetPermissionsForRole("Editor")),
                "Viewer" => new ViewerRole(permissionManager.GetPermissionsForRole("Viewer")),
                _ => throw new Exception("Unknown role!")
            };

            var newUser = new User(username, password, userRole);
            _users.Add(username, newUser);

            File.AppendAllLines(UsersFilePath, new[] { $"{username}:{password}:{role}" });
            Console.WriteLine("User registered successfully!");
        }
    }
}
