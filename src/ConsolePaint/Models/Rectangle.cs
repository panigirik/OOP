namespace ConsolePaint.Models
{
    public class Rectangle : Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(int width, int height, char borderChar, int x, int y)
        {
            Width = width;
            Height = height;
            BorderChar = borderChar;
            this.x = x;
            this.y = y;
        }

        public override void Draw()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (i == 0 || i == Height - 1 || j == 0 || j == Width - 1)
                        Console.Write(BorderChar);
                    else
                        Console.Write(' ');
                }
                Console.WriteLine();
            }
        }

        public override void DrawOnCanvas(char[,] canvas)
        {
            int startX = (canvas.GetLength(1) - Width) / 2;
            int startY = (canvas.GetLength(0) - Height) / 2;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (i == 0 || i == Height - 1 || j == 0 || j == Width - 1)
                    {
                        int x = startX + j;
                        int y = startY + i;
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