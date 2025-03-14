using ConsolePaint.Interfaces;

namespace ConsolePaint.Services
{
    public class FileService : IFileService
    {
        public void SaveCanvas(char[,] canvas)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "canvas.txt");

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    int rows = canvas.GetLength(0);
                    int cols = canvas.GetLength(1);

                    for (int i = 0; i < rows; i++)
                    {
                        bool hasDrawing = false;

                        for (int j = 0; j < cols; j++)
                        {
                            if (canvas[i, j] != ' ') 
                            {
                                writer.Write(canvas[i, j]);
                                hasDrawing = true;
                            }
                            else if (hasDrawing) 
                            {
                               writer.Write(' ');
                            }
                        }

                        if (hasDrawing) writer.WriteLine();
                    }
                }

                Console.WriteLine($"канва сохранена {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"не удалось сохранить канву: {ex.Message}");
            }
        }



        public char[,] LoadCanvas(string filePath, int canvasHeight, int canvasWidth)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("такой файл не найден");
                return null;
            }

            try
            {
                char[,] loadedCanvas = new char[canvasHeight, canvasWidth];

                for (int i = 0; i < canvasHeight; i++)
                {
                    for (int j = 0; j < canvasWidth; j++)
                    {
                        loadedCanvas[i, j] = ' ';
                    }
                }

                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < Math.Min(lines.Length, canvasHeight); i++)
                {
                    for (int j = 0; j < Math.Min(lines[i].Length, canvasWidth); j++)
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
}
