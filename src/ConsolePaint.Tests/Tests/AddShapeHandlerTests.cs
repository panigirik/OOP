using System.Reflection;
using ConsolePaint.Models;
using ConsolePaint.Services;
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
        
        public void AddShape(string shapeName, string shapeType, int x, int y, int width, int height)
        {
            char borderChar = '#';
            var shape = new Rectangle(width, height, borderChar, x, y, shapeName);
            
            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null)
                throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes == null)
                throw new Exception("'shapes' field is not a Dictionary<string, Shape>");

            shapes.Add(shapeName, shape);  
        }

        private bool ContainsShape()
        {
            var shapesField = typeof(DrawingService).GetField("shapes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shapesField == null) throw new Exception("Field 'shapes' not found in DrawingService");

            var shapes = shapesField.GetValue(_drawingService) as Dictionary<string, Shape>;
            if (shapes == null) throw new Exception("'shapes' field is not a Dictionary<string, Shape>");

            return shapes.Count > 0; 
        }

        [Fact]
        public void AddShape_ShouldAddShapeToDictionary()
        {
            var input = new StringReader("ShapeName\nrectangle\n10\n10\n5\n5\n#\n"); 
            Console.SetIn(input); 

            Console.SetIn(input); 

            Console.SetOut(new StringWriter()); 

            AddShape("ShapeName", "rectangle", 10, 10, 5, 5); 
            
            Assert.True(ContainsShape()); 

            Console.SetIn(new StringReader(string.Empty)); 
            Console.SetOut(new StringWriter()); 
        }
    }
}
