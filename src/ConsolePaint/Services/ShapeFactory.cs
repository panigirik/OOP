using ConsolePaint.Models;

namespace ConsolePaint.Services;

public class ShapeFactory
{
    public static Shape CreateShape(string type, int x, int y, string name, int width = 0, int height = 0, int radius = 0)
    {
        return type switch
        {
            "rectangle" => new Rectangle(width, height, '#', x, y),
            "circle" => new Circle(radius, '#', x, y),
            "triangle" => new Triangle(height, '#', x, y, width),
            _ => null,
        };
    }
}
