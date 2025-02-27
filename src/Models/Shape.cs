namespace ConsolePaint.Models;

public abstract class Shape
{
    public char FillCharacter { get; set; } = '*';
    public abstract void Draw(char[,] canvas, int canvasWidth, int canvasHeight);
    
    public abstract string Type { get; }
    
}