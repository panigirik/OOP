using Xunit;

namespace ConsoleWord.Tests.UseCases.UseCases
{
    public class AbstractTest
    {
        [Fact]
        public void AbstractTest_MethodShouldReturnTrue()
        {
            int value1 = 5;
            int value2 = 10;
            
            int result = value1 + value2;
            
            Assert.True(result == 15); 
        }
        
        [Fact]
        public void Concatenation_ShouldReturnCorrectString()
        {
            string part1 = "Hello";
            string part2 = "World";
            
            string result = part1 + " " + part2;
            
            Assert.Equal("Hello World", result); // Simply checks string concatenation
        }
        
        [Fact]
        public void StringLength_ShouldReturnCorrectLength()
        {
            string text = "Test String";

            // Act
            int length = text.Length;

            // Assert
            Assert.Equal(11, length); // Simply checks string length
        }
        
        [Fact]
        public void Substring_ShouldReturnCorrectPart()
        {
            // Arrange
            string text = "Hello, World!";

            // Act
            string result = text.Substring(0, 5);

            // Assert
            Assert.Equal("Hello", result); // Simply checks substring extraction
        }
    
            
        [Fact]
        public void ToUpper_ShouldConvertToUppercase()
        {
            // Arrange
            string text = "hello";

            // Act
            string result = text.ToUpper();

            // Assert
            Assert.Equal("HELLO", result); // Checks if the string is correctly converted to uppercase
        }
        
        [Fact]
        public void ToLower_ShouldConvertToLowercase()
        {
            // Arrange
            string text = "HELLO";

            // Act
            string result = text.ToLower();

            // Assert
            Assert.Equal("hello", result); // Checks if the string is correctly converted to lowercase
        }
    
        [Fact]
        public void ContainsSubstring_ShouldReturnTrueForMatching()
        {
            // Arrange
            string text = "Hello, World!";

            // Act
            bool result = text.Contains("World");

            // Assert
            Assert.True(result); // Checks if the string contains a substring
        }
        
        [Fact]
        public void StartsWith_ShouldReturnTrueForMatchingPrefix()
        {
            // Arrange
            string text = "Hello, World!";

            // Act
            bool result = text.StartsWith("Hello");

            // Assert
            Assert.True(result); // Checks if the string starts with a specific substring
        }
        
        [Fact]
        public void EndsWith_ShouldReturnTrueForMatchingSuffix()
        {
            // Arrange
            string text = "Hello, World!";

            // Act
            bool result = text.EndsWith("World!");

            // Assert
            Assert.True(result); // Checks if the string ends with a specific substring
        }
        

        
        [Fact]
        public void Trim_ShouldRemoveExtraSpaces()
        {
            // Arrange
            string text = "   Hello World!   ";

            // Act
            string result = text.Trim();

            // Assert
            Assert.Equal("Hello World!", result); // Checks if the string is trimmed of extra spaces
        }
        
        [Fact]
        public void Replace_ShouldCorrectlyReplaceSubstring()
        {
            // Arrange
            string text = "Hello, friend!";

            // Act
            string result = text.Replace("friend", "world");

            // Assert
            Assert.Equal("Hello, world!", result); // Checks if the substring is correctly replaced
        }
    }
        
        
}
        

    
    
        
