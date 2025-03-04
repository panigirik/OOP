using ConsolePaint.Models;

namespace ConsolePaint.Interfaces;

public interface IDrawingService
{
    List<Shape> Shapes { get; }
    char FillCharacter { get; set; }

    void DrawShape();
    void RenderAllShapes(int canvasWidth, int canvasHeight);
    void SetFillCharacter(char fillChar);
}