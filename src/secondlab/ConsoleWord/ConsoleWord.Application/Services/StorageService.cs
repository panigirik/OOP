using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;

namespace ConsoleWord.Infrastracture;

public class StorageService
{
    private readonly IStorageProvider _localStorage;
    private readonly ICloudStorageProvider _cloudStorage;

    public StorageService(IStorageProvider localStorage, ICloudStorageProvider cloudStorage)
    {
        _localStorage = localStorage;
        _cloudStorage = cloudStorage;
    }

    public void SaveToLocal(Document doc, string path)
    {
        _localStorage.Save(doc, path);
    }

    public Document LoadFromLocal(string path)
    {
        return _localStorage.Load(path);
    }

    public void UploadToCloud(Document doc, string cloudPath)
    {
        _cloudStorage.Upload(doc, cloudPath);
    }

    public Document DownloadFromCloud(string cloudPath)
    {
        return _cloudStorage.Download(cloudPath);
    }
}
