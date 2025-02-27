namespace ConsolePaint.Models;

public class Rectangle : Shape
{
    public override string Type => "Rectangle";
    public int Width { get; set; }
    public int Height { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public override void Draw(char[,] canvas, int canvasWidth, int canvasHeight)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int posX = X + x;
                int posY = Y + y;
            
                if (posX >= 0 && posX < canvasWidth && posY >= 0 && posY < canvasHeight)
                {
                    canvas[posY, posX] = FillCharacter;
                }
            }
        }
    }

}