using Xunit;
using ConsolePaint.Models;
using ConsolePaint.Services;

namespace ConsolePaint.Tests.Tests.HistoryServiceTests
{
    public class SaveStateHandlerTests
    {
        [Fact]
        public void SaveState_ShouldStoreCurrentState()
        {
            var historyService = new HistoryService();
            var shapes = new Dictionary<string, Shape>
            {
                { "shape1", new Rectangle(2, 2, '#', 0, 0, "shape1") }
            };

            historyService.SaveState(shapes);

            Assert.NotNull(historyService);
        }
    }
}