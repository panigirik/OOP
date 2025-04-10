using ConsoleWord.Application.Commands;

public class UndoRedoService
{
    private readonly Stack<IDocumentCommand> _undoStack = new();
    private readonly Stack<IDocumentCommand> _redoStack = new();

    public void ExecuteCommand(IDocumentCommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
        }
    }
}