using ConsoleWord.Core.Entities;

namespace ConsoleWord.Infrastracture.LocalStorage.Interfaces;

public interface IStorageProvider
{
    void Save(Document doc, string path);
    Document Load(string path);
}