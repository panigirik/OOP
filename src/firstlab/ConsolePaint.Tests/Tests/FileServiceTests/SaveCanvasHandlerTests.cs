using Xunit;
using ConsolePaint.Services;

namespace ConsolePaint.Tests.Tests.FileServiceTests
{
    public class SaveCanvasHandlerTests
    {
        [Fact]
        public void SaveCanvas_ShouldCreateFile()
        {
            var fileService = new FileService();
            char[,] canvas = { { '#', ' ' }, { ' ', '#' } };

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "canvas.txt");
            if (File.Exists(filePath))
                File.Delete(filePath);

            fileService.SaveCanvas(canvas);

            Assert.True(File.Exists(filePath));

       }
    }
}