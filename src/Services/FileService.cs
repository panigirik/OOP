using ConsolePaint.Interfaces;

public class FileService: IFileService
{
    private const string DefaultFileName = "DRAW.txt";
    
    public void SaveCanvas(char[,] canvas)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), DefaultFileName);
        
        try
        {
            int rows = canvas.GetLength(0);
            int cols = canvas.GetLength(1);
            Console.WriteLine($"Saving canvas to {filePath}. Canvas size: {rows}x{cols}");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(canvas[i, j] == '\0' ? ' ' : canvas[i, j]);
                    }
                    writer.WriteLine(); 
                }
            }

            Console.WriteLine($"Canvas successfully saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving canvas: {ex.Message}");
        }
    }

    public char[,] LoadCanvas(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: File not found.");
            return null;
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            int rows = lines.Length;
            int cols = rows > 0 ? lines[0].Length : 0;
            char[,] loadedCanvas = new char[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    loadedCanvas[i, j] = lines[i][j];
                }
            }

            Console.WriteLine("Canvas successfully loaded from file.");
            return loadedCanvas;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading canvas: {ex.Message}");
            return null;
        }
    }
}