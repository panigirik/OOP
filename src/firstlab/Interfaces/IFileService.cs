using ConsolePaint.Models;

namespace ConsolePaint.Interfaces;

public interface IFileService
{
    void SaveCanvas(char[,] canvas);

    char[,] LoadCanvas(string filePath, int canvasHeight, int canvasWidth);


}