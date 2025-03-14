using System.Reflection;
using ConsolePaint.Models;
using ConsolePaint.Services;
using Xunit;

namespace ConsolePaint.Tests.Tests.DrawingServiceTests
{
    public class MoveShapeHandlerTests
    {
        private DrawingService _drawingService;

        public MoveShapeHandlerTests()
        {
            _drawingService = DrawingService.Instance;

            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField != null)
            {
                var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
                shapes?.Clear();
            }
        }

        private void MoveShape(string name, int newX, int newY)
        {
            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null)
                throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes != null && shapes.ContainsKey(name))
            {
                var shape = shapes[name];
                shape.Move(newX, newY);
            }
        }

        [Fact]
        public void MoveShape_ShouldMoveSuccessfully()
        {
            var rectangle = new Rectangle(5, 3, '*', 10, 10, "TestRectangle");
            
            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null)
                throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes == null)
                throw new Exception("Shapes dictionary not found.");

            shapes["TestRectangle"] = rectangle;
            
            MoveShape("TestRectangle", 50, 50);
            
            Assert.Equal(50, rectangle.x);
            Assert.Equal(50, rectangle.y);
        }
    }
}
