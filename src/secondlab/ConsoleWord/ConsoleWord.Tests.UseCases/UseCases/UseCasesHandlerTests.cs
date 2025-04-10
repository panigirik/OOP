using Moq;
using ConsoleWord.Application.DocumentUseCases;
using Xunit;

namespace ConsoleWord.Tests.UseCases.UseCases
{
    public class UseCasesHandlerTests
    {
        private readonly Mock<DocumentEditor> _mockEditor;
        private readonly string _testFilePath;
        private readonly string _testText;

        public UseCasesHandlerTests()
        {
            _mockEditor = new Mock<DocumentEditor>();
            _testFilePath = "test.docx";
            _testText = "Sample text";
        }




        [Fact]
        public void LoadDocument_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var filePath = "nonexistent.docx";

            // Act & Assert
            var exception = Assert.Throws<FileNotFoundException>(() => _mockEditor.Object.LoadDocument(filePath));
            Assert.Equal($"Document not found at {filePath}", exception.Message);
        }





    }
}
