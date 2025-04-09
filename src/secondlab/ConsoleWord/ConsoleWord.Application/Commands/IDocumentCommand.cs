namespace ConsoleWord.Application.Commands;

public interface IDocumentCommand
{
    void Execute();
    void Undo();
}
