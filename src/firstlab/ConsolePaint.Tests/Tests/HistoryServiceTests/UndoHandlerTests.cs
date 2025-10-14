using Xunit;
using ConsolePaint.Models;
using ConsolePaint.Services;

namespace ConsolePaint.Tests.Tests.HistoryServiceTests
{
    public class UndoHandlerTests
    {
        [Fact]
        public void Undo_ShouldRevertToPreviousState()
        {
            var historyService = new HistoryService();
            var shapes = new Dictionary<string, Shape>
            {
                { "shape1", new Rectangle(2, 2, '#', 0, 0, "shape1") }
            };

            historyService.SaveState(shapes);
            shapes["shape2"] = new Rectangle(3, 3, '*', 1, 1, "shape2");

            historyService.Undo(ref shapes);

            
            Assert.False(shapes.ContainsKey("shape2")); 
            Assert.True(shapes.ContainsKey("shape1")); 
        }
    }
}