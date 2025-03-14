    namespace ConsolePaint.Models;

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
            int startX = (canvas.GetLength(1) - Width) / 2;
            int startY = (canvas.GetLength(0) - Height) / 2;

            int fillStartX = startX + 1;
            int fillStartY = startY + 1;

            if (fillStartX >= canvas.GetLength(1) || fillStartY >= canvas.GetLength(0))
                return;

            if (canvas[fillStartY, fillStartX] != ' ')
                return;

            FloodFill(canvas, fillStartX, fillStartY, fillChar);
        }
        
        public override bool IsInside(int testX, int testY)
        {
            return testX >= x && testX < x + Width && testY >= y && testY < y + Height;
        }

        public override string SaveToString()
        {
            return $"Rectangle:{Name},{x},{y},{Width},{Height},{BorderChar}";
        }

    }