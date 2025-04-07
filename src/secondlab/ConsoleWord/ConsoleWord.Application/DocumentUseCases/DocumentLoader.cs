using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = ConsoleWord.Core.Entities.Document;

namespace ConsoleWord.Application.DocumentUseCases;

public class DocumentLoader
{


    public Document LoadDocx(string filePath)
    {
        using var wordDoc = WordprocessingDocument.Open(filePath, false);
        var body = wordDoc.MainDocumentPart.Document.Body;

        var text = new StringBuilder();
        foreach (var paragraph in body.Elements<Paragraph>())
        {
            foreach (var run in paragraph.Elements<Run>())
            {
                text.Append(run.InnerText);
            }
            text.AppendLine();
        }

        return new Document(Path.GetFileNameWithoutExtension(filePath), text.ToString(), "Arial", 12);
    }

    public Document LoadXml(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Document));
        using var reader = new StreamReader(filePath);
        return (Document)serializer.Deserialize(reader);
    }


    public Document LoadJson(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Document>(json);
    }
}
