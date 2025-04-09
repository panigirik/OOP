using ConsoleWord.Core.Entities;

namespace ConsoleWord.Infrastracture.CloudStorage.Interfaces;

public interface ICloudOperationsStorageProvider
{
    void Upload(Document doc, string cloudPath);
    Document Download(string cloudPath);
}
