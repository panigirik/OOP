namespace ConsoleWord.Application.Dropbox;

    public interface ICloudDropBoxStorageProvider
    {
        Task UploadFileAsync(string fileName, byte[] content);
        Task<byte[]> DownloadFileAsync(string fileName);
    }
