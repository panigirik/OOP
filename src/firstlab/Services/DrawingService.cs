using ConsolePaint.Interfaces;
using ConsolePaint.Models;

namespace ConsolePaint.Services;

public class DrawingService: IDrawingService
{
    private static DrawingService _instance;
    private Dictionary<string, Shape> shapes = new();
    private HistoryService historyService = new();
    private const int CanvasWidth = 150;
    private const int CanvasHeight = 150;
    private char[,] _canvas = new char[CanvasHeight, CanvasWidth];

    private DrawingService()
    {
        InitializeCanvas();
    }

    public static DrawingService Instance => _instance ??= new DrawingService();

    private void InitializeCanvas()
    {
        for (int i = 0; i < CanvasHeight; i++)
        {
            for (int j = 0; j < CanvasWidth; j++)
            {
                _canvas[i, j] = ' ';
            }
        }
    }

    public void DrawCanvas(bool saveToFile = false)
    {
        Console.Clear();
        Console.WriteLine("+" + new string('-', CanvasWidth) + "+");
        for (int i = 0; i < CanvasHeight; i++)
        {
            Console.Write("|");
            for (int j = 0; j < CanvasWidth; j++)
            {
                Console.Write(_canvas[i, j]);
            }
            Console.WriteLine("|");
        }
        Console.WriteLine("+" + new string('-', CanvasWidth) + "+");
    }

    public void AddShape()
    {
        Console.Write("введите имя фигруы: ");
        string name = Console.ReadLine();
        Console.Write("введите тип фигуры (rectangle, circle, triangle): ");
        string type = Console.ReadLine().ToLower();
        Console.Write("введите x: ");
        int x = int.Parse(Console.ReadLine());
        Console.Write("введите y: ");
        int y = int.Parse(Console.ReadLine());

        Shape shape;

        switch (type)
        {
            case "rectangle":
                Console.Write("введите ширину: ");
                int width = int.Parse(Console.ReadLine());
                Console.Write("Enter height: ");
                int height = int.Parse(Console.ReadLine());
                Console.Write("введите символ границы: ");
                char rectBorder = Console.ReadKey().KeyChar;
                Console.WriteLine();
                shape = new Rectangle(width, height, rectBorder, x, y, name);
                break;
            case "circle":
                Console.Write("введите радиус: ");
                int radius = int.Parse(Console.ReadLine());
                Console.Write("введите символ границы: ");
                char circBorder = Console.ReadKey().KeyChar;
                Console.WriteLine();
                shape = new Circle(radius, circBorder, x, y);
                break;
            case "triangle":
                Console.Write("введите высоту: ");
                int triHeight = int.Parse(Console.ReadLine());
                Console.Write("Enter width: ");
                int triWidth = int.Parse(Console.ReadLine());
                Console.Write("введите символ границы: ");
                char triBorder = Console.ReadKey().KeyChar;
                Console.WriteLine();
                shape = new Triangle(triHeight, triBorder, x, y, triWidth, name); 
                break;

            default:
                Console.WriteLine("несуществующий тип фигуры");
                return;
        }
        historyService.SaveState(new Dictionary<string, Shape>(shapes)); 
        shapes[name] = shape;
        RedrawCanvas(true);
        }


    public Shape GetShape(string name)
    {
        if (shapes.ContainsKey(name))
            return shapes[name];
        return null;
    }

    
    public void RemoveShape(string name)
    {
        if (shapes.Remove(name))
        {
            RedrawCanvas();
        }
        else
        {
            Console.WriteLine("фигура не найдена");
        }
    }

    public void MoveShape()
    {
        Console.Write("введите название фигуры, для перемещения ");
        string name = Console.ReadLine();
        if (!shapes.ContainsKey(name))
        {
            Console.WriteLine("фигура не найдена");
            return;
        }

        Console.Write("введите новую x: ");
        int newX = int.Parse(Console.ReadLine());
        Console.Write("введите новую y: ");
        int newY = int.Parse(Console.ReadLine());

        Shape shape = shapes[name];
        shape.Move(newX, newY);


        historyService.SaveState(new Dictionary<string, Shape>(shapes)); 
        RedrawCanvas();
    }

    public void FillShape(string shapeName, char fillChar)
    {
        if (!shapes.ContainsKey(shapeName))
        {
            Console.WriteLine("Shape not found.");
            return;
        }

        Shape shape = shapes[shapeName];

        for (int y = 0; y < CanvasHeight; y++)
        {
            for (int x = 0; x < CanvasWidth; x++)
            {
                if (shape.IsInside(x, y)) 
                {
                    _canvas[y, x] = fillChar;
                }
            }
        }

        DrawCanvas();
    }


    public void LoadFromFile()
    {
        FileService fileService = new FileService();
        Console.Write("Введите путь к файлу: ");
        string filePath = Console.ReadLine();
        char[,] loadedCanvas = fileService.LoadCanvas(filePath, CanvasHeight, CanvasWidth);

        if (loadedCanvas != null)
        {
            _canvas = loadedCanvas;
            DrawCanvas();
        }
    }



    
    public void Undo()
    {
        historyService.Undo(ref shapes); 
        RedrawCanvas();
    }

    public void Redo()
    {
        historyService.Redo(ref shapes); 
        RedrawCanvas();
    }

    
    public void Save(bool saveToFile = true)
    {
        historyService.Redo(ref shapes); 
        if (saveToFile)
        {
            FileService fileService = new FileService();
            fileService.SaveCanvas(_canvas);
        }
    }


    public void RedrawCanvas(bool saveToFile = true)
    {
        InitializeCanvas();
        foreach (var shape in shapes.Values)
        {
            shape.DrawOnCanvas(_canvas);
        }
        DrawCanvas(saveToFile);
    }
}