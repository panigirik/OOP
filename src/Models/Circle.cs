namespace ConsolePaint.Models;

public class Circle : Shape
{
    
    public override string Type => "Circle";
    public int Radius { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public override void Draw(char[,] canvas, int canvasWidth, int canvasHeight)
    {
        for (int i = -Radius; i <= Radius; i++)
        {
            for (int j = -Radius; j <= Radius; j++)
            {
                if (Math.Pow(i, 2) + Math.Pow(j, 2) <= Math.Pow(Radius, 2))
                {
                    int posX = X + i;
                    int posY = Y + j;
                    if (posX >= 0 && posX < canvasWidth && posY >= 0 && posY < canvasHeight)
                    {
                        canvas[posY, posX] = FillCharacter;
                    }
                }
            }
        }
    }

}
