using System.Text;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;

namespace ConsoleWord.Infrastracture.CloudStorage.Services
{
    public class CloudFileStorage 
    {
        // Метод для загрузки документа в облако
        public void UploadDocument(Document doc, string cloudPath)
        {
            Console.WriteLine($"Uploading document '{doc.Name}' to {cloudPath}...");
            // Загрузка в облачное хранилище
            Upload(doc, cloudPath); // Используем вашу реализацию загрузки
        }

        // Метод для скачивания документа из облака
        public Document DownloadDocument(string cloudPath)
        {
            Console.WriteLine($"Downloading document from {cloudPath}...");
            // Скачиваем документ из облачного хранилища
            return Download(cloudPath); // Используем вашу реализацию загрузки
        }

        // Приватные методы для работы с облаком
        public void Upload(Document doc, string cloudPath)
        {
            // Здесь логика загрузки в облачное хранилище
            Console.WriteLine($"Document '{doc.Name}' uploaded to {cloudPath}.");
        }

        public Document Download(string cloudPath)
        {
            // Здесь логика загрузки из облака
            Console.WriteLine($"Document downloaded from {cloudPath}.");
            // Пример документа
            return new PlainTextDocument("CloudDocument") { Content = new StringBuilder("Sample cloud content") };
        }
    }
}