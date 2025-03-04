namespace ConsolePaint.Models
{
    public class Circle : Shape
    {
        public int Radius { get; set; }

        public Circle(int radius, char borderChar, int x, int y)
        {
            Radius = radius;
            BorderChar = borderChar;
            this.x = x;
            this.y = y;
        }

        public override void Draw()
        {
            for (int i = -Radius; i <= Radius; i++)
            {
                for (int j = -Radius; j <= Radius; j++)
                {
                    if (i * i + j * j <= Radius * Radius)
                        Console.Write(BorderChar);
                    else
                        Console.Write(' ');
                }
                Console.WriteLine();
            }
        }

        public override void DrawOnCanvas(char[,] canvas)
        {
            int centerX = canvas.GetLength(1) / 2;
            int centerY = canvas.GetLength(0) / 2;

            for (int i = -Radius; i <= Radius; i++)
            {
                for (int j = -Radius; j <= Radius; j++)
                {
                    if (i * i + j * j <= Radius * Radius)
                    {
                        int x = centerX + j;
                        int y = centerY + i;
                        if (x >= 0 && x < canvas.GetLength(1) && y >= 0 && y < canvas.GetLength(0))
                        {
                            canvas[y, x] = BorderChar;
                        }
                    }
                }
            }
        }
    }
}