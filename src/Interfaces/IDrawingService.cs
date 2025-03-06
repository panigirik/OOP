
namespace ConsolePaint.Interfaces;

public interface IDrawingService
{
    void DrawCanvas();

    void AddShape();

    void RemoveShape(string name);

    void MoveShape();

    void FillShape(string shapeName, char fillChar);
}