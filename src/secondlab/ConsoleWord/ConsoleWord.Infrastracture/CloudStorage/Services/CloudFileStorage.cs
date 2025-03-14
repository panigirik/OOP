using System.Text;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;

namespace ConsoleWord.Infrastracture.CloudStorage.Services;

public class CloudFileStorage : ICloudStorageProvider
{
    public void Upload(Document doc, string cloudPath)
    {
        Console.WriteLine($"Uploading document '{doc.Name}' to {cloudPath}...");
        // Здесь может быть логика загрузки в облачное хранилище
    }

    public Document Download(string cloudPath)
    {
        Console.WriteLine($"Downloading document from {cloudPath}...");
        // Здесь заглушка, в реальности загрузка данных
        return new PlainTextDocument("CloudDocument") { Content = new StringBuilder("Sample cloud content") };
    }
}
