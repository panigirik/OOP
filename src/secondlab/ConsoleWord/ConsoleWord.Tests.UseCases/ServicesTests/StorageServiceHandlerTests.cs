using System.IO;
using System.Text;
using System.Threading.Tasks;
using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using ConsoleWord.Application.Dropbox;
using Moq;
using Xunit;

namespace ConsoleWord.Tests.UseCases.ServicesTests;

public class StorageServiceHandlerTests
{
    private readonly Mock<IStorageProvider> _localStorageMock;
    private readonly Mock<ICloudDropBoxStorageProvider> _cloudStorageMock;

    public StorageServiceHandlerTests()
    {
        _localStorageMock = new Mock<IStorageProvider>();
        _cloudStorageMock = new Mock<ICloudDropBoxStorageProvider>();
    }
    

    [Fact]
    public async Task UploadToCloudAsync_UnsupportedFormat_DoesNotUpload()
    {
        var document = new Document { Name = "InvalidDoc", Content = new StringBuilder("Content") };
        var service = new StorageService(_localStorageMock.Object, _cloudStorageMock.Object);
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        Console.SetOut(writer);
        await service.UploadToCloudAsync(document, "exe");
        _cloudStorageMock.Verify(cs => cs.UploadFileAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        Assert.Contains("Unsupported format", output.ToString());
    }


}
