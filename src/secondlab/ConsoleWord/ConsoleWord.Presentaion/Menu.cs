using System;
using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;

namespace ConsoleWord
{
    public class Menu
    {
        private readonly DocumentService _documentService;
        private readonly IStorageProvider _localStorage;
        private readonly ICloudStorageProvider _cloudStorage;
        private readonly AuthenticationService _authenticationService;

        private User _currentUser;

        public Menu(DocumentService documentService, IStorageProvider localStorage, ICloudStorageProvider cloudStorage, AuthenticationService authenticationService)
        {
            _documentService = documentService;
            _localStorage = localStorage;
            _cloudStorage = cloudStorage;
            _authenticationService = authenticationService;
        }

        public void Show()
        {
            // Запрашиваем аутентификацию перед показом меню
            ShowAuthenticationOptions();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== Document Editor ====");
                Console.WriteLine("Welcome, " + _currentUser.Username);
                Console.WriteLine("1. Create new document");
                Console.WriteLine("2. Open and edit document");
                Console.WriteLine("3. Logout");
                Console.WriteLine("4. Exit");
                Console.Write("Choose option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (_currentUser.Role.HasPermission("Edit"))
                        {
                            _documentService.CreateAndSaveDocument();
                        }
                        else
                        {
                            Console.WriteLine("You don't have permission to create a document.");
                        }
                        break;
                    case "2":
                        if (_currentUser.Role.HasPermission("Read"))
                        {
                            _documentService.OpenAndEditDocument();
                        }
                        else
                        {
                            Console.WriteLine("You don't have permission to edit documents.");
                        }
                        break;
                    case "3":
                        Logout();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }


        private void ShowAuthenticationOptions()
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Document Editor!");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    AuthenticateUser();
                    break;
                }
                else if (choice == "2")
                {
                    RegisterUser();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option, please try again.");
                }
            }
        }

        private void AuthenticateUser()
        {
            Console.WriteLine("Please log in to continue.");

            while (true)
            {
                Console.Write("Username: ");
                string username = Console.ReadLine();

                Console.Write("Password: ");
                string password = Console.ReadLine();

                _currentUser = _authenticationService.Authenticate(username, password);

                if (_currentUser != null)
                {
                    Console.WriteLine($"Welcome, {_currentUser.Username}! You are logged in as {_currentUser.Role.RoleName}.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid credentials. Please try again.");
                }
            }
        }

        private void RegisterUser()
        {
            Console.WriteLine("Please register to create a new account.");

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            Console.Write("Role (Admin/Editor/Viewer): ");
            string role = Console.ReadLine();

            _authenticationService.Register(username, password, role);

            // После регистрации выполняем аутентификацию
            AuthenticateUser();
        }

        private void Logout()
        {
            _currentUser = null;
            Console.WriteLine("You have been logged out.");
            ShowAuthenticationOptions();
        }
    }
}
