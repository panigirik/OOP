namespace ConsolePaint.Models
{
    public class Triangle : Shape
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Triangle(int height, char borderChar, int x, int y, int width)
        {
            Height = height;
            BorderChar = borderChar;
            Width = width;
            this.x = x;
            this.y = y;
        }
        

        public override void Draw()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= y * 2; x++)
                {
                    Console.Write(BorderChar);
                }
                Console.WriteLine();
            }
        }

        public override void DrawOnCanvas(char[,] canvas)
        {
            // Implement drawing logic for Triangle on canvas
        }
    }
}