using System.Text.Json;
using ConsoleWord.Core.Roles;

namespace ConsoleWord.Application.Helpers;

public class PermissionManager
{
    private const string PermissionsFilePath = "permissions.json";
    private Dictionary<string, List<string>> _rolePermissions;

    public PermissionManager()
    {
        LoadPermissions();
    }

    private void LoadPermissions()
    {
        if (!File.Exists(PermissionsFilePath))
        {
            _rolePermissions = new Dictionary<string, List<string>>
            {
                { "Editor", new List<string> { "Read", "Edit" } },
                { "Viewer", new List<string> { "Read" } }
            };
            SavePermissions();
        }
        else
        {
            var json = File.ReadAllText(PermissionsFilePath);
            _rolePermissions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                               ?? new Dictionary<string, List<string>>();
        }
    }

    private void SavePermissions()
    {
        var json = JsonSerializer.Serialize(_rolePermissions, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(PermissionsFilePath, json);
    }

    public void ManagePermissions()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Permission Management ===");
            Console.WriteLine("Available Roles: Editor, Viewer");
            Console.Write("Enter role to manage (or 'exit' to return): ");
            string role = Console.ReadLine();

            if (role?.ToLower() == "exit")
                break;

            if (!_rolePermissions.ContainsKey(role))
            {
                Console.WriteLine("Invalid role.");
                continue;
            }

            Console.WriteLine($"Current permissions for {role}: {string.Join(", ", _rolePermissions[role])}");
            Console.WriteLine("Choose action: 1 - Add Permission, 2 - Remove Permission, 3 - View Permissions");
            Console.Write("Your choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter permission to add: ");
                string perm = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(perm) && !_rolePermissions[role].Contains(perm))
                {
                    _rolePermissions[role].Add(perm);
                    SavePermissions();
                    Console.WriteLine("Permission added.");
                }
                else
                {
                    Console.WriteLine("Invalid or duplicate permission.");
                }
            }
            else if (choice == "2")
            {
                Console.Write("Enter permission to remove: ");
                string perm = Console.ReadLine();
                if (_rolePermissions[role].Remove(perm))
                {
                    SavePermissions();
                    Console.WriteLine("Permission removed.");
                }
                else
                {
                    Console.WriteLine("Permission not found.");
                }
            }
            else if (choice == "3")
            {
                Console.WriteLine($"Permissions for {role}: {string.Join(", ", _rolePermissions[role])}");
            }
            else
            {
                Console.WriteLine("Unknown action.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public List<string> GetPermissionsForRole(string role)
    {
        return _rolePermissions.TryGetValue(role, out var perms) ? perms : new List<string>();
    }
}
