using FluentAssertions;
using Moq;
using StudManager.Application.Interfaces;
using StudManager.Application.Adapters;
using StudManager.Application.Responses;
using Xunit;

namespace Tests
{
    public class QuoteAPIHandlerTest
    {
        [Fact]
        public async Task GetMotivationalQuoteAsync_ApiReturnsResponse_MapsToDto()
        {
            // Arrange
            var apiResponse = new QuoteApiResponse
            {
                Content = "Stay hungry, stay foolish",
                Author  = "Steve Jobs"
            };
            
            var apiMock = new Mock<IExternalQuoteApi>();
            apiMock
                .Setup(a => a.FetchRandomQuoteAsync())
                .ReturnsAsync(apiResponse);
            
            var adapter = new QuoteAdapter(apiMock.Object);
            
            // Act
            var dto = await adapter.GetMotivationalQuoteAsync();
            
            // Assert
            dto.Should().NotBeNull();
            dto!.Content.Should().Be(apiResponse.Content);
            dto.Author.Should().Be(apiResponse.Author);
        }
        
        [Fact]
        public async Task GetMotivationalQuoteAsync_ApiReturnsNull_ReturnsNull()
        {
            // Arrange
            var apiMock = new Mock<IExternalQuoteApi>();
            apiMock
                .Setup(a => a.FetchRandomQuoteAsync())
                .ReturnsAsync((QuoteApiResponse?)null);
            
            var adapter = new QuoteAdapter(apiMock.Object);
            
            // Act
            var dto = await adapter.GetMotivationalQuoteAsync();
            
            // Assert
            dto.Should().BeNull();
        }
    }
}