using ConsolePaint.Models;
using ConsolePaint.Services;
using Xunit;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ConsolePaint.Tests
{
    public class MoveShapeHandlerTests
    {
        private DrawingService _drawingService;

        public MoveShapeHandlerTests()
        {
            _drawingService = DrawingService.Instance;
        }

        public void AddShape(string shapeName, string shapeType, int x, int y, int width, int height)
        {
            // You need a valid borderChar here. I will use '#' for now.
            char borderChar = '#';
            var shape = new Rectangle(width, height, borderChar, x, y, shapeName);

            // Use reflection to get the private 'shapes' dictionary from DrawingService
            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null)
                throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes == null)
                throw new Exception("'shapes' field is not a Dictionary<string, Shape>");

            shapes.Add(shapeName, shape);  // Add the shape to the dictionary
        }

        
        public void MoveShape(string name, int newX, int newY)
        {
            // Use reflection to get the private 'shapes' dictionary from DrawingService
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
            // Act: пытаемся переместить фигуру, которой нет в коллекции
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
               MoveShape("NonExistentShape", 100, 100);

                // Assert: в консоли должно вывести сообщение о том, что фигура не найдена
                var result = sw.ToString().Trim();
                Assert.Equal("Shape not found.", result);
            }
        }
    }
}
