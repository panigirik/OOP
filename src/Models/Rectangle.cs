   using ConsolePaint.Models;

   public class Rectangle : Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(int width, int height, char borderChar, int x, int y, string name)
        {
            Width = width;
            Height = height;
            BorderChar = borderChar;
            this.x = x;
            this.y = y;
            Name = name;
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
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int canvasX = x + j;
                    int canvasY = y + i;

                    if (canvasX >= 0 && canvasX < canvas.GetLength(1) && canvasY >= 0 && canvasY < canvas.GetLength(0))
                    {
                        if (i == 0 || i == Height - 1 || j == 0 || j == Width - 1)
                        {
                            canvas[canvasY, canvasX] = BorderChar;
                        }
                    }
                }
            }
        }

        public override void Fill(char[,] canvas, char fillChar)
        {
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    int canvasX = x + j;
                    int canvasY = y + i;

                    if (canvasX >= 0 && canvasX < canvas.GetLength(1) && canvasY >= 0 && canvasY < canvas.GetLength(0))
                    {
                        canvas[canvasY, canvasX] = fillChar;
                    }
                }
            }
        }
    }
