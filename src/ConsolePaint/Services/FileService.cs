using System;
using System.IO;
using ConsolePaint.Interfaces;

public class FileService : IFileService
{
    public void SaveCanvas(char[,] canvas, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            int rows = canvas.GetLength(0);
            int cols = canvas.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    writer.Write(canvas[i, j] == '\0' ? ' ' : canvas[i, j]);  // Write space if no shape
                }
                writer.WriteLine();
            }
        }

        Console.WriteLine($"Canvas successfully saved to {filePath}");
    }

    public char[,] LoadCanvas(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        int height = lines.Length;
        int width = lines[0].Length;
        char[,] canvas = new char[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                canvas[i, j] = lines[i][j];
            }
        }

        Console.WriteLine($"Canvas successfully loaded from {filePath}");
        return canvas;
    }
}