using ConsolePaint.Models;
using ConsolePaint.Services;
using Xunit;
using System.IO;
using System;
using System.Reflection;

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
            // Arrange - No shape added yet
            string shapeName = "NonExistentShape";
            char fillChar = '*';

            // Act - Try to fill a non-existent shape
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                _drawingService.FillShape(shapeName, fillChar);

                // Assert - The console should display "Shape not found."
                var result = sw.ToString().Trim();
                Assert.Equal("Shape not found.", result);
            }
        }
    }
}
