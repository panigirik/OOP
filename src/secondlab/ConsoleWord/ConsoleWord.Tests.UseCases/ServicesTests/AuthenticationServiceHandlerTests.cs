using ConsoleWord.Application.Services;
using ConsoleWord.Core.Roles;
using Xunit;

namespace ConsoleWord.Tests.UseCases.ServicesTests
{
    public class AuthenticationServiceHandlerTests : IDisposable
    {
        private readonly string _testFilePath = "users.txt";

        public AuthenticationServiceHandlerTests()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [Fact]
        public void Authenticate_ReturnsUser_WhenCredentialsAreCorrect()
        {
            File.WriteAllText(_testFilePath, "john:123:Admin");
            var authService = new AuthenticationService();
            
            var user = authService.Authenticate("john", "123");
            
            Assert.NotNull(user);
            Assert.Equal("john", user.Username);
            Assert.Equal("123", user.Password);
            Assert.IsType<AdminRole>(user.Role);
        }

        [Fact]
        public void Authenticate_ReturnsNull_WhenPasswordIsIncorrect()
        {
            File.WriteAllText(_testFilePath, "john:123:Admin");
            var authService = new AuthenticationService();
            
            var user = authService.Authenticate("john", "wrongpassword");
            
            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_ReturnsNull_WhenUserNotExists()
        {
            File.WriteAllText(_testFilePath, "john:123:Admin");
            var authService = new AuthenticationService();
            
            var user = authService.Authenticate("notfound", "123");
            
            Assert.Null(user);
        }

        [Fact]
        public void Register_AddsUser_WhenUserIsNew()
        {
            var sw = new StringWriter();
            Console.SetOut(sw);

            var authService = new AuthenticationService();
            
            authService.Register("newuser", "pass", "Viewer");
            
            var user = authService.Authenticate("newuser", "pass");
            Assert.NotNull(user);
            Assert.Equal("newuser", user.Username);
            Assert.IsType<ViewerRole>(user.Role);
            Assert.Contains("User registered successfully!", sw.ToString());
        }

        [Fact]
        public void Register_DoesNotAddUser_WhenUserAlreadyExists()
        {
            File.WriteAllText(_testFilePath, "john:123:Admin");
            var sw = new StringWriter();
            Console.SetOut(sw);

            var authService = new AuthenticationService();
            
            authService.Register("john", "123", "Admin");
            
            Assert.Contains("User already exists!", sw.ToString());
        }

        [Fact]
        public void LoadUsers_FileNotExists_PrintsWarning()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);

            var sw = new StringWriter();
            Console.SetOut(sw);
            
            var authService = new AuthenticationService();
            
            Assert.Contains("User data file not found!", sw.ToString());
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }
    }
}
