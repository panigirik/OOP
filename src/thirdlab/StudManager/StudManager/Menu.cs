using StudManager.Application.Creators;
using StudManager.Application.DTOs;
using StudManager.Application.Interfaces;

namespace StudManager;

public class Menu
{
    //private readonly IAddStudentAsyncCommand _addStudentAsyncCommand;
    private readonly IGetAllStudentsAsyncCommand _getAllStudentsAsyncCommand;
    private readonly IEditStudentAsyncCommand _editStudentAsyncCommand;
    private readonly StudentCreator _studentCreator;
    public Menu(IGetAllStudentsAsyncCommand getAllStudentsAsyncCommand,
        IEditStudentAsyncCommand editStudentAsyncCommand,
        StudentCreator studentCreator)
    {
        _getAllStudentsAsyncCommand = getAllStudentsAsyncCommand;
        _editStudentAsyncCommand = editStudentAsyncCommand;
        _studentCreator = studentCreator;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine("\n=== Student Management Menu ===");
            Console.WriteLine("1. Add student");
            Console.WriteLine("2. List all students");
            Console.WriteLine("3. Edit student");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await AddStudentAsync();
                    break;
                case "2":
                    await ListStudentsAsync();
                    break;
                case "3":
                    await EditStudentAsync();
                    break;
                case "0":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("[!] Invalid option.");
                    break;
            }
        }
    }

    private async Task AddStudentAsync()
    {
        Console.Write("Enter student name: ");
        var name = Console.ReadLine()?.Trim();

        Console.Write("Enter student grade: ");
        var gradeStr = Console.ReadLine();

        if (!int.TryParse(gradeStr, out var grade))
        {
            Console.WriteLine("[!] Invalid grade.");
            return;
        }

        await _studentCreator.CreateAsync(new StudentDTO
        {
            Name = name!,
            Grade = grade
        });

        Console.WriteLine("[+] Student added.");
    }

    private async Task ListStudentsAsync()
    {
        var students = await _getAllStudentsAsyncCommand.GetAllStudentsAsync();

        if (students.Count == 0)
        {
            Console.WriteLine("[!] No students found.");
            return;
        }

        Console.WriteLine("\n--- Students ---");
        for (int i = 0; i < students.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {students[i].Name} — Grade: {students[i].Grade}");
        }
    }

    private async Task EditStudentAsync()
    {
        Console.Write("Enter student ID (GUID): ");
        var idStr = Console.ReadLine();

        if (!Guid.TryParse(idStr, out var id))
        {
            Console.WriteLine("[!] Invalid GUID.");
            return;
        }

        Console.Write("New name: ");
        var name = Console.ReadLine();

        Console.Write("New grade: ");
        var gradeStr = Console.ReadLine();

        if (!int.TryParse(gradeStr, out var grade))
        {
            Console.WriteLine("[!] Invalid grade.");
            return;
        }

        try
        {
            await _editStudentAsyncCommand.EditStudentAsync(id, new StudentDTO
            {
                Name = name!,
                Grade = grade
            });

            Console.WriteLine("[~] Student updated.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[!] Error: {ex.Message}");
        }
    }
}
