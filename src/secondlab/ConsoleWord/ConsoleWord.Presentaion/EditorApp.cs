using ConsoleWord.Application.Services;
using ConsoleWord.Application.Roles;
using ConsoleWord.Application.Notifications;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Storage;

namespace ConsoleWord.Presentation
{
    public class EditorApp
    {
        private readonly DocumentService _documentService;
        private readonly CloudFileStorage _cloudStorage;
        private readonly LocalFileStorage _localStorage;
        private readonly ConsoleNotification _notification;
        private IUserRole _currentUserRole;
        private readonly Menu _menu;

        public EditorApp()
        {
            var storageService = DependencyFactory.GetStorageService();
            var formattingService = DependencyFactory.GetFormattingService();
            _menu = DependencyFactory.GetMenu();

            _documentService = new DocumentService(storageService, formattingService);
            _cloudStorage = new CloudFileStorage();
            _localStorage = new LocalFileStorage();
            _notification = new ConsoleNotification();
            _currentUserRole = new ViewerRole();
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Document Editor!");
            bool exit = false;

            while (!exit)
            {
                _menu.Show();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateDocument();
                        break;
                    case "2":
                        OpenDocument();
                        break;
                    case "3":
                        EditDocument();
                        break;
                    case "4":
                        SaveDocument();
                        break;
                    case "5":
                        ChangeUserRole();
                        break;
                    case "6":
                        Console.WriteLine("Exiting application...");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private void CreateDocument()
        {
            Console.WriteLine("Enter document type (plain, markdown, rich): ");
            string type = Console.ReadLine();

            Document document = type switch
            {
                "plain" => new PlainTextDocument("NewPlainDoc"),
                "markdown" => new MarkdownDocument("NewMarkdownDoc"),
                "rich" => new RichTextDocument("NewRichDoc"),
                _ => null
            };

            if (document == null)
            {
                Console.WriteLine("Invalid document type.");
                return;
            }

            if (_currentUserRole is EditorRole || _currentUserRole is AdminRole)
            {
                _documentService.SaveDocumentLocally(document, "default_path.txt");
                _notification.Notify($"New {type} document created.");
            }
            else
            {
                Console.WriteLine("You do not have permission to create a document.");
            }
        }


        private void OpenDocument()
        {
            Console.WriteLine("Enter document name to open: ");
            string name = Console.ReadLine();
            var document = _localStorage.Load(name) ?? _cloudStorage.Download(name);

            if (document != null)
            {
                Console.WriteLine($"Document '{name}' loaded.");
            }
            else
            {
                Console.WriteLine("Document not found.");
            }
        }

        private void EditDocument()
        {
            if (!(_currentUserRole is EditorRole || _currentUserRole is AdminRole))
            {
                Console.WriteLine("You do not have permission to edit documents.");
                return;
            }

            Console.WriteLine("Enter document name to edit: ");
            string name = Console.ReadLine();
            var document = _localStorage.Load(name) ?? _cloudStorage.Download(name);
            
            if (document != null)
            {
                var updatedDocument = _documentService.FormatBold(document);
                _documentService.SaveDocumentLocally(updatedDocument, name);
                _notification.Notify($"Document '{name}' edited and saved.");
            }
            else
            {
                Console.WriteLine("Document not found.");
            }
        }

        private void SaveDocument()
        {
            Console.WriteLine("Enter document name to save: ");
            string name = Console.ReadLine();
            Console.WriteLine("Enter document path to save: ");
            string path = Console.ReadLine();

            // Загружаем документ из локального хранилища
            Document document = _localStorage.Load(name) ?? _cloudStorage.Download(name);

            if (document == null)
            {
                Console.WriteLine("Document not found.");
                return;
            }

            _localStorage.Save(document, path);
            _notification.Notify($"Document '{name}' saved at '{path}'.");
        }


        private void ChangeUserRole()
        {
            Console.WriteLine("Select role: 1 - Viewer, 2 - Editor, 3 - Admin");
            string role = Console.ReadLine();

            _currentUserRole = role switch
            {
                "1" => new ViewerRole(),
                "2" => new EditorRole(),
                "3" => new AdminRole(),
                _ => _currentUserRole
            };

            Console.WriteLine($"Role changed to: {_currentUserRole.GetType().Name}");
        }
    }
}