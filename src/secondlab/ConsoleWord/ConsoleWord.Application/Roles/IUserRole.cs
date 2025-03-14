using ConsoleWord.Core.Entities;

namespace ConsoleWord.Application.Roles;

public interface IUserRole
{
    void AccessDocument(Document doc);
}
