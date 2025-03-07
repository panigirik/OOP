using ConsolePaint.Services;

namespace ConsolePaint
{
    static class Program
    {
        static void Main()
        {
            var drawingService = DrawingService.Instance;
            char[,] canvas = new char[150, 150];
            
            drawingService.DrawCanvas();
            
            for (int i = 0; i < canvas.GetLength(0); i++)
            {
                for (int j = 0; j < canvas.GetLength(1); j++)
                {
                    canvas[i, j] = ' ';
                }
            }
            
            while (true)
            {
                Console.WriteLine("меню:");
                Console.WriteLine("1. добавить фигуру");
                Console.WriteLine("2. удалить фигуру");
                Console.WriteLine("3. отмена");
                Console.WriteLine("4. повтор");
                Console.WriteLine("5. сохранить канву");
                Console.WriteLine("6. загрузить канву");
                Console.WriteLine("7. выход");
                Console.WriteLine("8. передвинуть фигуру");
                Console.WriteLine("9. залить фигуру символом");

                Console.Write("выберите действие: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        drawingService.AddShape();
                        break;

                    case "2":
                        Console.Write("введите имя фигуры, которую хотите удалить: ");
                        drawingService.RemoveShape(Console.ReadLine());
                        break;

                    case "3":
                        drawingService.Undo();
                        break;

                    case "4":
                        drawingService.Redo();
                        break;

                    case "5":
                        drawingService.Save();
                        break;

                    case "6":
                        drawingService.LoadFromFile();
                        break;

                    case "7":
                        return;

                    case "8":
                        drawingService.MoveShape();
                        break;

                    case "9":
                        Console.WriteLine("введите имя фигуры, которую хотите залить: ");
                        string shapeName = Console.ReadLine();
                        Console.WriteLine("введите символ для заливки: ");
                        char fillChar;

                        string inputChar = Console.ReadLine();
                        if (!string.IsNullOrEmpty(inputChar) && inputChar.Length == 1)
                        {
                            fillChar = inputChar[0];
                            drawingService.FillShape(shapeName, fillChar);
                        }
                        else
                        {
                            Console.WriteLine("введите один конкретный символ");
                        }
                        break;

                    default:
                        Console.WriteLine("неверный ввод, попробуйте снова");
                        break;
                }
            }
        }
    }
}
