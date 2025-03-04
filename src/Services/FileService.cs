using System;
using System.IO;

public class FileService
{
    private const string DefaultFileName = "DRAW.txt";

    // Сохраняем содержимое канвы в файл
    public void SaveCanvas(char[,] canvas)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), DefaultFileName);
        
        try
        {
            // Логируем информацию о канве перед записью
            int rows = canvas.GetLength(0);
            int cols = canvas.GetLength(1);
            Console.WriteLine($"Saving canvas to {filePath}. Canvas size: {rows}x{cols}");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Пройдем по всей канве и записываем в файл
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        // Записываем каждый символ канвы или пробел, если символ пустой
                        writer.Write(canvas[i, j] == '\0' ? ' ' : canvas[i, j]);
                    }
                    writer.WriteLine(); // Каждая строка канвы переходит на новую строку в файле
                }
            }

            Console.WriteLine($"Canvas successfully saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving canvas: {ex.Message}");
        }
    }

    // Загружаем канву из файла
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

            // Загружаем данные из файла в канву
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