using System.Text;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;

namespace ConsoleWord.Infrastracture.LocalStorage.Storage;

public class LocalFileStorage : IStorageProvider
{
    public void Save(Document doc, string path)
    {
        File.WriteAllText(path, doc.ToString());
    }

    public Document Load(string path)
    {
        string content = File.ReadAllText(path);
        return new PlainTextDocument(Path.GetFileNameWithoutExtension(path)) { Content = new StringBuilder(content) };
    }
}