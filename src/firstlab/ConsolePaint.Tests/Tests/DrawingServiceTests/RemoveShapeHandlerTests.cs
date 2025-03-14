using ConsolePaint.Services;
using Xunit;

namespace ConsolePaint.Tests.Tests.DrawingServiceTests
{
    public class RemoveShapeHandlerTests
    {
        private DrawingService _drawingService;

        public RemoveShapeHandlerTests()
        {
            _drawingService = DrawingService.Instance;
        }


        [Fact]
        public void RemoveShape_ShouldNotRemoveWhenShapeNotFound()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                _drawingService.RemoveShape("NonExistentShape");
                
                var result = sw.ToString().Trim();
                Assert.Equal("фигура не найдена", result);
            }
        }
    }
}
