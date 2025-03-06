namespace ConsolePaint.Interfaces;

public interface IFileService
{
    char[,] LoadCanvas(string filePath);

    void SaveCanvas(char[,] canvas);
    
    
}