using ConsolePaint.Services;
using ConsolePaint.Models;

namespace ConsolePaint
{
    class Program
    {
        static void Main()
        {
            DrawingService drawingService = DrawingService.Instance;
            FileService fileService = new FileService();
            HistoryService historyService = new HistoryService();

            // Get canvas dimensions from the user
            Console.Write("Введите ширину канвы: ");
            int canvasWidth = int.Parse(Console.ReadLine());
            Console.Write("Введите высоту канвы: ");
            int canvasHeight = int.Parse(Console.ReadLine());

            // Initialize the canvas
            char[,] canvas = new char[canvasHeight, canvasWidth];  // Initialize the canvas

            while (true)
            {
                Console.WriteLine("\nКонсольный пэинт");
                Console.WriteLine("1. Нарисовать фигуру");
                Console.WriteLine("2. Сохранить в файл");
                Console.WriteLine("3. Загрузить из файла");
                Console.WriteLine("4. Отмена (Ctrl+Z)");
                Console.WriteLine("5. Повтор (Ctrl+Shift+Z)");
                Console.WriteLine("12. Залить символом");
                Console.WriteLine("6. Выйти");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        drawingService.DrawShape();
                        historyService.SaveState(canvas);  // Передаем состояние канвы для сохранения
                        break;
                    case "2":
                        Console.Write("Введите путь для сохранения файла: ");
                        string savePath = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(savePath))
                        {
                            fileService.SaveCanvas(canvas, savePath);  // Сохранение канвы в файл
                            historyService.SaveState(canvas);  // Сохранение канвы в историю
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: путь не указан.");
                        }
                        break;
                    case "3":
                        Console.Write("Введите путь к файлу для загрузки: ");
                        string loadPath = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(loadPath) && File.Exists(loadPath))
                        {
                            // Загрузка канвы из файла
                            canvas = fileService.LoadCanvas(loadPath);
                            drawingService.RenderAllShapes(canvasWidth, canvasHeight);  // Рендер канвы на экране после загрузки
                            historyService.SaveState(canvas);  // Сохраняем состояние после загрузки
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: файл не найден или путь пуст.");
                        }
                        break;
                    case "4":
                        historyService.Undo(ref canvas);  // Передаем ссылку на канву для отмены
                        break;
                    case "5":
                        historyService.Redo(ref canvas);  // Передаем ссылку на канву для повторения
                        break;

                }

                Console.WriteLine("\nТекущие фигуры:");
                drawingService.RenderAllShapes(canvasWidth, canvasHeight);
                // Ensure this method renders the current canvas to the screen
            }
        }
    }
}
