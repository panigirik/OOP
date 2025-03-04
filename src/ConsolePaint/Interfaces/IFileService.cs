namespace ConsolePaint.Interfaces;

public interface IFileService
{
    void SaveCanvas(char[,] canvas, string filePath);
    char[,] LoadCanvas(string filePath);
}