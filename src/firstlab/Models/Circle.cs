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
                    int distance = i * i + j * j;
                    if (distance <= Radius * Radius && distance >= (Radius - 1) * (Radius - 1))
                        Console.Write(BorderChar);
                    else
                        Console.Write(' ');
                }
                Console.WriteLine();
            }
        }

        public override void DrawOnCanvas(char[,] canvas)
        {
            int centerX = x;
            int centerY = y;

            for (int i = -Radius; i <= Radius; i++)
            {
                for (int j = -Radius; j <= Radius; j++)
                {
                    int distance = i * i + j * j;
                    if (distance <= Radius * Radius && distance >= (Radius - 1) * (Radius - 1))
                    {
                        int drawX = centerX + j;
                        int drawY = centerY + i;
                        if (drawX >= 0 && drawX < canvas.GetLength(1) && drawY >= 0 && drawY < canvas.GetLength(0))
                        {
                            canvas[drawY, drawX] = BorderChar;
                        }
                    }
                }
            }
        }
        
        public override bool IsInside(int testX, int testY)
        {
            int dx = testX - x;
            int dy = testY - y;
            return (dx * dx + dy * dy <= Radius * Radius);
        }


        public override void Fill(char[,] canvas, char fillChar)
        {
            int centerX = x;
            int centerY = y;

            for (int i = -Radius; i <= Radius; i++)
            {
                for (int j = -Radius; j <= Radius; j++)
                {
                    if (i * i + j * j < Radius * Radius)
                    {
                        int fillX = centerX + j;
                        int fillY = centerY + i;
                        if (fillX >= 0 && fillX < canvas.GetLength(1) && fillY >= 0 && fillY < canvas.GetLength(0))
                        {
                            canvas[fillY, fillX] = fillChar;
                        }
                    }
                }
            }
        }
        
        public override string SaveToString()
        {
            return $"Rectangle:{Name},{x},{y},{BorderChar}"; // переписать потом
        }
        
    }
}
