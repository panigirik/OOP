namespace ConsolePaint.Interfaces;

public interface IHistoryService
{
    void SaveState(char[,] currentState);
    void Undo(ref char[,] canvas);
    void Redo(ref char[,] canvas);
}