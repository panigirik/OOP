using ConsolePaint.Models;
using ConsolePaint.Services;

public class DrawingService
{
    private static DrawingService _instance;
    private Dictionary<string, Shape> shapes = new();
    private HistoryService historyService = new();
    private const int CanvasWidth = 150;
    private const int CanvasHeight = 150;
    private char[,] canvas = new char[CanvasHeight, CanvasWidth];

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
                canvas[i, j] = ' ';
            }
        }
    }

 public void DrawCanvas()
    {
        Console.Clear();
        Console.WriteLine("+" + new string('-', CanvasWidth) + "+");
        for (int i = 0; i < CanvasHeight; i++)
        {
            Console.Write("|");
            for (int j = 0; j < CanvasWidth; j++)
            {
                Console.Write(canvas[i, j]);
            }
            Console.WriteLine("|");
        }
        Console.WriteLine("+" + new string('-', CanvasWidth) + "+");
    }

    public void AddShape()
    {
        Console.Write("Enter shape name: ");
        string name = Console.ReadLine();
        Console.Write("Select shape type (rectangle, circle, triangle): ");
        string type = Console.ReadLine().ToLower();
        Console.Write("Enter X coordinate: ");
        int x = int.Parse(Console.ReadLine());
        Console.Write("Enter Y coordinate: ");
        int y = int.Parse(Console.ReadLine());

        Shape shape = null;

        switch (type)
        {
            case "rectangle":
                Console.Write("Enter width: ");
                int width = int.Parse(Console.ReadLine());
                Console.Write("Enter height: ");
                int height = int.Parse(Console.ReadLine());
                Console.Write("Enter border character: ");
                char rectBorder = Console.ReadKey().KeyChar;
                Console.WriteLine();
                shape = new Rectangle(width, height, rectBorder, x, y, name);
                break;
            case "circle":
                Console.Write("Enter radius: ");
                int radius = int.Parse(Console.ReadLine());
                Console.Write("Enter border character: ");
                char circBorder = Console.ReadKey().KeyChar;
                Console.WriteLine();
                shape = new Circle(radius, circBorder, x, y);
                break;
            case "triangle":
                Console.Write("Enter height: ");
                int triHeight = int.Parse(Console.ReadLine());
                Console.Write("Enter width: ");
                int triWidth = int.Parse(Console.ReadLine());
                Console.Write("Enter border character: ");
                char triBorder = Console.ReadKey().KeyChar;
                Console.WriteLine();
                shape = new Triangle(triHeight, triBorder, x, y, triWidth, name); // Убедитесь, что тут создается Triangle, а не Circle
                break;

            default:
                Console.WriteLine("Invalid shape type.");
                return;
        }
        historyService.SaveState(new Dictionary<string, Shape>(shapes)); 
        shapes[name] = shape;
        RedrawCanvas();
    }


    public void RemoveShape(string name)
    {
        if (shapes.Remove(name))
        {
            RedrawCanvas();
        }
        else
        {
            Console.WriteLine("Shape not found.");
        }
    }

    public void MoveShape()
    {
        Console.Write("Enter the shape name to move: ");
        string name = Console.ReadLine();
        if (!shapes.ContainsKey(name))
        {
            Console.WriteLine("Shape not found.");
            return;
        }

        Console.Write("Enter new X coordinate: ");
        int newX = int.Parse(Console.ReadLine());
        Console.Write("Enter new Y coordinate: ");
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
                    canvas[y, x] = fillChar;
                }
            }
        }

        DrawCanvas();
    }




    
    public void Undo()
    {
        historyService.Undo(ref shapes); // передаем всю коллекцию shapes в Undo
        RedrawCanvas();
    }

    public void Redo()
    {
        historyService.Redo(ref shapes); // передаем всю коллекцию shapes в Redo
        RedrawCanvas();
    }




    public void RedrawCanvas()
    {
        InitializeCanvas();
        foreach (var shape in shapes.Values)
        {
            shape.DrawOnCanvas(canvas);
        }
        DrawCanvas();
    }
    }
