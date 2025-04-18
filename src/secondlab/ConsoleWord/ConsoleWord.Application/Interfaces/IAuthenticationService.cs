using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Interfaces
{
    public interface IAuthenticationService
    {
        User Authenticate(string username, string password);
        void Register(string username, string password, string role);
    }
}