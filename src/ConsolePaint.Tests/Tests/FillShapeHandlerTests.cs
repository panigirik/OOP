using ConsolePaint.Services;
using Xunit;


namespace ConsolePaint.Tests
{
    public class FillShapeHandlerTests
    {
        private DrawingService _drawingService;

        public FillShapeHandlerTests()
        {
            _drawingService = DrawingService.Instance;
        }

        [Fact]
        public void FillShape_ShouldNotFillNonExistentShape()
        {
            string shapeName = "NonExistentShape";
            char fillChar = '*';
            
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                _drawingService.FillShape(shapeName, fillChar);

                var result = sw.ToString().Trim();
                Assert.Equal("Shape not found.", result);
            }
        }
    }
}
