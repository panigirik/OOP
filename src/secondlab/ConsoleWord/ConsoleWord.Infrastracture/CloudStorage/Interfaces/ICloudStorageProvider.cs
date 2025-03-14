using ConsoleWord.Core.Entities;

namespace ConsoleWord.Infrastracture.CloudStorage.Interfaces;

public interface ICloudStorageProvider
{
    void Upload(Document doc, string cloudPath);
    Document Download(string cloudPath);
}
