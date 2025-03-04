namespace ConsolePaint.Models
{
    public abstract class Shape
    {
        public char BorderChar { get; set; }
        public abstract void Draw();
        public abstract void DrawOnCanvas(char[,] canvas);
        
        public void Move(int newX, int newY)
        {
            x = newX;
            y = newY;
        }
        
        public string Name { get; set; }

        public int x { get; set; }
        public int y { get; set; }
    }
}