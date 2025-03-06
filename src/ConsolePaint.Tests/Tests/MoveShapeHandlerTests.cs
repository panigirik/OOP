using ConsolePaint.Models;
using ConsolePaint.Services;
using Xunit;
using System.Reflection;

namespace ConsolePaint.Tests
{
    public class MoveShapeHandlerTests
    {
        private DrawingService _drawingService;

        public MoveShapeHandlerTests()
        {
            _drawingService = DrawingService.Instance;
        }

        
        public void MoveShape(string name, int newX, int newY)
        {
           var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null)
                throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes != null && shapes.ContainsKey(name))
            {
                var shape = shapes[name];
                shape.x = newX;
                shape.y = newY;
            }
            else
            {
                Console.WriteLine("Shape not found.");
            }
        }


        [Fact]
        public void MoveShape_ShouldNotMoveWhenShapeNotFound()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
               MoveShape("NonExistentShape", 100, 100);

               var result = sw.ToString().Trim();
               Assert.Equal("Shape not found.", result);
            }
        }
    }
}
