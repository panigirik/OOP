namespace ConsolePaint
{
    class Program
    {
        static void Main()
        {
            var drawingService = DrawingService.Instance;
            FileService fileService = new FileService();

            // Инициализация канвы (например, 150x150)
            char[,] canvas = new char[150, 150];

            // Заполним канву символами для теста
            for (int i = 0; i < canvas.GetLength(0); i++)
            {
                for (int j = 0; j < canvas.GetLength(1); j++)
                {
                    canvas[i, j] = '.';
                }
            }

            // Отображаем канву для проверки
            drawingService.DrawCanvas();

            while (true)
            {
                Console.WriteLine("Console Paint Menu:");
                Console.WriteLine("1. Add Shape");
                Console.WriteLine("2. Remove Shape");
                Console.WriteLine("3. Undo Action");
                Console.WriteLine("4. Redo Action");
                Console.WriteLine("5. Save Canvas");
                Console.WriteLine("6. Load Canvas");
                Console.WriteLine("7. Exit");
                Console.WriteLine("8. Move Shape");
                Console.WriteLine("9. Fill Shape");
                
                Console.Write("Select an option: ");

                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        drawingService.AddShape();
                        break;
                    case "2":
                        Console.Write("Enter the shape name to remove: ");
                        drawingService.RemoveShape(Console.ReadLine());
                        break;
                    case "3":
                        drawingService.Undo();
                        break;
                    case "4":
                        drawingService.Redo();
                        break;
                    case "5":
                        // Передаем канву в сохранение
                        fileService.SaveCanvas(canvas);
                        break;
                    case "6":
                        Console.Write("Enter the file path: ");
                        string filePath = Console.ReadLine();
                        // Загружаем канву из файла
                        char[,] loadedCanvas = fileService.LoadCanvas(filePath);
                        if (loadedCanvas != null)
                        {
                            canvas = loadedCanvas; // Обновляем канву
                        }
                        break;
                    case "7":
                        return;
                    case "8":
                        drawingService.MoveShape();
                        break;
                    case "9":
                        drawingService.FillShape();
                        break;
                    default:
                        Console.WriteLine("Invalid input, try again.");
                        break;
                }
            }
        }
    }
}