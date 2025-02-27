using ConsolePaint.Models;
using ConsolePaint.Interfaces;

public class DrawingService: IDrawingService
{
    public List<Shape> Shapes { get; set; } = new List<Shape>();
    public char FillCharacter { get; set; } = '*'; // Default fill character

    private static DrawingService _instance;
    
    private DrawingService() 
    {
        Shapes = new List<Shape>();
    }
    
    public static DrawingService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DrawingService();
            }
            return _instance;
        }
    }

    public void DrawShape()
    {
        Console.WriteLine("Выберите фигуру (1-Круг, 2-Прямоугольник, 3-Треугольник): ");
        string choice = Console.ReadLine();
    
        // Prompt for the fill character before creating the shape
        Console.Write("Введите символ для рисования: ");
        char fillChar = Console.ReadKey().KeyChar;
        Console.WriteLine();  // Move to the next line

        Shape shape = choice switch
        {
            "1" => CreateCircle(fillChar),
            "2" => CreateRectangle(fillChar),
            "3" => CreateTriangle(fillChar),
            _ => null
        };

        if (shape == null)
        {
            Console.WriteLine("Неверный выбор.");
            return;
        }

        Shapes.Add(shape);
        Console.Clear();
    }


    private (int, int) GetCenterCoordinates()
    {
        Console.Write("Введите координату X центра: ");
        int x = int.Parse(Console.ReadLine());
        Console.Write("Введите координату Y центра: ");
        int y = int.Parse(Console.ReadLine());
        return (x, y);
    }

    private Circle CreateCircle(char fillChar)
    {
        Console.Write("Введите радиус круга: ");
        int radius = int.Parse(Console.ReadLine());
        var (x, y) = GetCenterCoordinates();
        return new Circle { Radius = radius, X = x, Y = y, FillCharacter = fillChar };
    }

    private Rectangle CreateRectangle(char fillChar)
    {
        Console.Write("Введите ширину прямоугольника: ");
        int width = int.Parse(Console.ReadLine());
        Console.Write("Введите высоту прямоугольника: ");
        int height = int.Parse(Console.ReadLine());
        var (x, y) = GetCenterCoordinates();
        return new Rectangle { Width = width, Height = height, X = x, Y = y, FillCharacter = fillChar };
    }

    private Triangle CreateTriangle(char fillChar)
    {
        Console.Write("Введите высоту треугольника: ");
        int height = int.Parse(Console.ReadLine());
        var (x, y) = GetCenterCoordinates();
        return new Triangle { Height = height, X = x, Y = y, FillCharacter = fillChar };
    }


    public void RenderAllShapes(int canvasWidth, int canvasHeight)
    {
        // Очищаем перед отрисовкой
        char[,] canvas = new char[canvasHeight, canvasWidth];

        // Заполняем пустыми символами
        for (int i = 0; i < canvasHeight; i++)
        {
            for (int j = 0; j < canvasWidth; j++)
            {
                canvas[i, j] = ' ';
            }
        }

        // Рисуем все фигуры в буфере
        foreach (var shape in Shapes)
        {
            shape.Draw(canvas, canvasWidth, canvasHeight); 
        }

        // Выводим "холст" с фигурами
        for (int i = 0; i < canvasHeight; i++)
        {
            for (int j = 0; j < canvasWidth; j++)
            {
                Console.Write(canvas[i, j]);
            }
            Console.WriteLine();
        }

        // Разделительная линия
        Console.WriteLine(new string('-', canvasWidth));

        // Выводим меню
        Console.WriteLine("Консольный пэинт");
        Console.WriteLine("1. Нарисовать фигуру");
        Console.WriteLine("2. Сохранить в файл");
        Console.WriteLine("3. Загрузить из файла");
        Console.WriteLine("4. Отмена (Ctrl+Z)");
        Console.WriteLine("5. Повтор (Ctrl+Shift+Z)");
        Console.WriteLine("6. Выйти");
        Console.Write("Выберите действие: ");
    }


    public void SetFillCharacter(char fillChar)
    {
        FillCharacter = fillChar;  // Update the global fill character
    }
}