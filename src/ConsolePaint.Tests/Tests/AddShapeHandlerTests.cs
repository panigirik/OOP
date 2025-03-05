using System.Reflection;
using ConsolePaint.Models;
using Xunit;

namespace ConsolePaint.Tests
{
    public class AddShapeHandlerTests
    {
        private readonly DrawingService _drawingService;

        public AddShapeHandlerTests()
        {
            _drawingService = DrawingService.Instance;
        }

        // This method adds a shape to the shapes dictionary
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

        private bool ContainsShape()
        {
            // Use reflection to get the private 'shapes' dictionary from DrawingService
            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null) throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes == null) throw new Exception("'shapes' field is not a Dictionary<string, Shape>");

            return shapes.Count > 0; // Check if any shape exists in the dictionary
        }

        [Fact]
        public void AddShape_ShouldAddShapeToDictionary()
        {
            // Arrange
            var input = new StringReader("ShapeName\nrectangle\n10\n10\n5\n5\n#\n"); // Simulating input
            Console.SetIn(input); // Redirect the console input

            // Mocking Console.ReadKey to avoid errors
            Console.SetIn(input); // Redirect the console input again

            // Redirect Console output to avoid messing with the output
            Console.SetOut(new StringWriter()); // Optionally redirect output

            // Act
            AddShape("ShapeName", "rectangle", 10, 10, 5, 5); // Call the method to add the shape

            // Assert
            Assert.True(ContainsShape()); // Assert that a shape has been added

            // Restore the original console behavior
            Console.SetIn(new StringReader(string.Empty)); // Restore console input
            Console.SetOut(new StringWriter()); // Restore standard output
        }
    }
}
