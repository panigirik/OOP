namespace ConsoleWord.Application.Interfaces;

public interface ICommand
{
    void Execute();
    void Undo();
}