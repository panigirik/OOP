using ConsolePaint.Models;
public class Triangle : Shape
{
    public override string Type => "Triangle";
    public int Height { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public override void Draw(char[,] canvas, int canvasWidth, int canvasHeight)
    {
        for (int y = 0; y < Height; y++)
        {
            int startX = X - y;
            int endX = X + y;
        
            for (int x = startX; x <= endX; x++)
            {
                if (x >= 0 && x < canvasWidth && Y + y >= 0 && Y + y < canvasHeight)
                {
                    canvas[Y + y, x] = FillCharacter;
                }
            }
        }
    }

}