namespace ConsolePaint.Models
{
    public class Triangle : Shape
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Triangle(int height, char borderChar, int x, int y, int width, string name)
        {
            Height = height;
            BorderChar = borderChar;
            Width = width;
            this.x = x;
            this.y = y;
            Name = name;
        }

        public override void Draw()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= y * 2; x++)
                {
                    if (y == Height - 1 || x == 0 || x == y * 2)
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
            int startY = y;

            for (int i = 0; i < Height; i++)
            {
                int startX = centerX - i;
                int endX = centerX + i;

                for (int j = startX; j <= endX; j++)
                {
                    if (i == Height - 1 || j == startX || j == endX)
                    {
                        if (j >= 0 && j < canvas.GetLength(1) && (startY + i) >= 0 &&
                            (startY + i) < canvas.GetLength(0))
                        {
                            canvas[startY + i, j] = BorderChar;
                        }
                    }
                }
            }
        }

        public override void Fill(char[,] canvas, char fillChar)
        {
            int centerX = x;
            int startY = y;

            for (int i = 0; i < Height; i++)
            {
                int startX = centerX - i;
                int endX = centerX + i;

                for (int j = startX; j <= endX; j++)
                {
                    if (j >= 0 && j < canvas.GetLength(1) && (startY + i) >= 0 && (startY + i) < canvas.GetLength(0))
                    {
                        canvas[startY + i, j] = fillChar;
                    }
                }
            }
        }
    }
}