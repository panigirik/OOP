using ConsoleWord.Application.Commands;
using Moq;
using Xunit;

namespace ConsoleWord.Tests.UseCases.ServicesTests;

public class UndoRedoServiceHandlerTests
{
    [Fact]
    public void ExecuteCommand_ExecutesAndPushesToUndoStack()
    {
        var commandMock = new Mock<IDocumentCommand>();
        var service = new UndoRedoService();

        service.ExecuteCommand(commandMock.Object);

        commandMock.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void Undo_WhenUndoStackHasCommand_UndoesAndMovesToRedoStack()
    {
        var commandMock = new Mock<IDocumentCommand>();
        var service = new UndoRedoService();
        service.ExecuteCommand(commandMock.Object);

        service.Undo();

        commandMock.Verify(c => c.Undo(), Times.Once);
    }

    [Fact]
    public void Undo_WhenUndoStackIsEmpty_DoesNothing()
    {
        var service = new UndoRedoService();

        var exception = Record.Exception(() => service.Undo());

        Assert.Null(exception);
    }

    [Fact]
    public void Redo_WhenRedoStackHasCommand_ExecutesAndMovesBackToUndoStack()
    {
        var commandMock = new Mock<IDocumentCommand>();
        var service = new UndoRedoService();
        service.ExecuteCommand(commandMock.Object);
        service.Undo();

        service.Redo();

        commandMock.Verify(c => c.Execute(), Times.Exactly(2)); // initial + redo
    }

    [Fact]
    public void Redo_WhenRedoStackIsEmpty_DoesNothing()
    {
        var service = new UndoRedoService();

        var exception = Record.Exception(() => service.Redo());

        Assert.Null(exception);
    }

    [Fact]
    public void ExecuteCommand_ClearsRedoStack()
    {
        var firstCommand = new Mock<IDocumentCommand>();
        var secondCommand = new Mock<IDocumentCommand>();
        var service = new UndoRedoService();

        service.ExecuteCommand(firstCommand.Object);
        service.Undo();
        service.ExecuteCommand(secondCommand.Object);

        service.Redo(); 

        firstCommand.Verify(c => c.Execute(), Times.Once); 
    }
}
