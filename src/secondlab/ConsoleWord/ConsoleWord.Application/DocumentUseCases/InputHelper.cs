namespace ConsoleWord.Application.DocumentUseCases;

public class InputHelper
{
    
    public string GetUserInput(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()?.Trim();
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input. Please try again.");
            Console.Write(prompt);
            input = Console.ReadLine()?.Trim();
        }
        return input;
    }
    
    
    public bool AskYesNo(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()?.Trim().ToLower();

        while (input != "y" && input != "n")
        {
            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            Console.Write(prompt);
            input = Console.ReadLine()?.Trim().ToLower();
        }

        return input == "y";
    }
    
    

    public int GetValidatedInteger(string prompt)
    {
        int value;
        Console.Write(prompt);
        while (!int.TryParse(Console.ReadLine(), out value) || value <= 0)
        {
            Console.WriteLine("Invalid number. Please enter a positive integer.");
            Console.Write(prompt);
        }
        return value;
    }
    
    public string GetValidDirectory(string prompt)
    {
        Console.Write(prompt);
        string directory = Console.ReadLine()?.Trim();
        while (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            Console.WriteLine("Invalid directory. Please enter a valid path.");
            Console.Write(prompt);
            directory = Console.ReadLine()?.Trim();
        }
        return directory;
    }
}