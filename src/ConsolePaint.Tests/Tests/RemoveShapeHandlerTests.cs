using ConsolePaint.Models;
using ConsolePaint.Services;
using Xunit;
using System;
using System.IO;
using System.Reflection;

namespace ConsolePaint.Tests
{
    public class RemoveShapeHandlerTests
    {
        private DrawingService _drawingService;

        public RemoveShapeHandlerTests()
        {
            _drawingService = DrawingService.Instance;
        }

        
        // Helper method to add a shape
        public void AddShape(string shapeName, string shapeType, int x, int y, int width, int height)
        {
            char borderChar = '#';
            var shape = new Rectangle(width, height, borderChar, x, y, shapeName);

            // Use reflection to get the private 'shapes' dictionary from DrawingService
            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null)
                throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes == null)
                throw new Exception("'shapes' field is not a Dictionary<string, Shape>");

            shapes.Add(shapeName, shape);
        }


        [Fact]
        public void RemoveShape_ShouldNotRemoveWhenShapeNotFound()
        {
            // Act: Attempt to remove a shape that doesn't exist
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                _drawingService.RemoveShape("NonExistentShape");

                // Assert: The console should display "Shape not found."
                var result = sw.ToString().Trim();
                Assert.Equal("Shape not found.", result);
            }
        }
    }
}
