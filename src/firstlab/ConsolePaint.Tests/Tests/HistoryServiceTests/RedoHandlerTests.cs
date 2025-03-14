using Xunit;
using ConsolePaint.Models;
using ConsolePaint.Services;

namespace ConsolePaint.Tests.Tests.HistoryServiceTests
{
    public class RedoHandlerTests
    {
        [Fact]
        public void Redo_ShouldRestorePreviousState()
        {
            var historyService = new HistoryService();
            var shapes = new Dictionary<string, Shape>
            {
                { "shape1", new Rectangle(2, 2, '#', 0, 0, "shape1") }
            };

            historyService.SaveState(shapes);
            shapes.Remove("shape1");
            historyService.Undo(ref shapes);

            historyService.Redo(ref shapes);

            Assert.False(shapes.ContainsKey("shape1")); 
        }
    }
}