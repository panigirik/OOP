
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using StudManager.Application.Creators;
using StudManager.Application.DTOs;
using StudManager.Application.Interfaces;
using StudManager.Application.ValidationInterfaces;
using StudManger.Domain.Entities;
using StudManger.Domain.Interfaces;
using Xunit;

namespace Tests
{
    public class AddStudentHandlerTest
    {
        [Fact]
        public async Task CreateAsync_ValidDto_SavesStudentWithQuote()
        {
            // Arrange
            var dto = new StudentDTO { Name = "Alice", Grade = 5 };
            
            var validatorMock = new Mock<IStudentValidationService>();
            validatorMock
                .Setup(v => v.ValidateLoginRequestAsync(dto))
                .ReturnsAsync(new ValidationResult());
            
            var quote = new QuoteDTO { Content = "Hello", Author = "Bob" };
            var quoteServiceMock = new Mock<IQuoteService>();
            quoteServiceMock
                .Setup(q => q.GetMotivationalQuoteAsync())
                .ReturnsAsync(quote);
            
            Student? saved = null;
            var repoMock = new Mock<IStudentRepository>();
            repoMock
                .Setup(r => r.AddAsync(It.IsAny<Student>()))
                .Returns(Task.CompletedTask)
                .Callback<Student>(s => saved = s);
            
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(m => m.Map(dto, It.IsAny<Student>()))
                .Callback<StudentDTO, Student>((sDto, s) => {
                    // обычно AutoMapper сам скопирует поля,
                    // здесь дублируем Name/Grade
                    s.Name = sDto.Name;
                    s.Grade = sDto.Grade;
                });
            
            var factory = new DefaultStudentFactory(
                repoMock.Object,
                quoteServiceMock.Object,
                mapperMock.Object,
                validatorMock.Object
            );
            
            // Act
            var result = await factory.CreateAsync(dto);
            
            // Assert
            // — репозиторий был вызван ровно один раз
            repoMock.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Once);
            // — сохранённый студент соответствует ожиданиям
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Alice");
            saved.Grade.Should().Be(5);
            saved.Quote.Should().Be("Hello — Bob");
            saved.Id.Should().NotBe(Guid.Empty);
            
            // — возвращён тот же объект, что и сохранённый
            result.Should().Be(saved);
        }
        
        
        [Fact]
        public async Task CreateAsync_NoQuote_SavesWithoutQuote()
        {
            // Arrange
            var dto = new StudentDTO { Name = "Bob", Grade = 7 };
            
            var validatorMock = new Mock<IStudentValidationService>();
            validatorMock
                .Setup(v => v.ValidateLoginRequestAsync(dto))
                .ReturnsAsync(new ValidationResult());
            
            var quoteServiceMock = new Mock<IQuoteService>();
            quoteServiceMock
                .Setup(q => q.GetMotivationalQuoteAsync())
                .ReturnsAsync((QuoteDTO?)null);
            
            Student? saved = null;
            var repoMock = new Mock<IStudentRepository>();
            repoMock
                .Setup(r => r.AddAsync(It.IsAny<Student>()))
                .Returns(Task.CompletedTask)
                .Callback<Student>(s => saved = s);
            
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(m => m.Map(dto, It.IsAny<Student>()));
            
            var factory = new DefaultStudentFactory(
                repoMock.Object,
                quoteServiceMock.Object,
                mapperMock.Object,
                validatorMock.Object
            );
            
            // Act
            var result = await factory.CreateAsync(dto);
            
            // Assert
            saved.Should().NotBeNull();
            saved!.Quote.Should().BeNullOrEmpty();
        }
    }
}
