using ConsoleWord.Core.Entities;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using Path = System.IO.Path;

namespace ConsoleWord.Application.DocumentUseCases;

public class DocumentEditor
{
    
    public void AppendTextToDocx(string path, string textToAppend)
    {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;

            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            run.AppendChild(new Text(textToAppend));
            paragraph.Append(run);

            body.Append(paragraph);
            wordDoc.MainDocumentPart.Document.Save();
        }
    }


    public void ClearDocxContent(string path)
    {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;
            body.RemoveAllChildren();
            wordDoc.MainDocumentPart.Document.Save();
        }
    }



    public string ReadDocxContent(string path)
    {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, false))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;
            return body.InnerText;
        }
    }
    
    public Document LoadDocument(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Document not found at {filePath}");
        }
        
        string documentText = ReadDocxContent(filePath);
        
        string documentName = Path.GetFileNameWithoutExtension(filePath);
        
        var document = new Document(documentName, documentText, "Arial", 12); 
        return document;
    }

    public void OverwriteTextFile(string path, string newText)
    {
        File.WriteAllText(path, newText);
    }

    public void OverwriteDocxFile(string path, string newText)
    {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;
            body.RemoveAllChildren(); 

            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            run.AppendChild(new Text(newText));
            paragraph.Append(run);

            body.Append(paragraph);
            wordDoc.MainDocumentPart.Document.Save();
        }
    }

    
    public string ReadTextFileContent(string path)
    {
        return File.ReadAllText(path);
    }
    
    public void AppendTextToTextFile(string path, string textToAppend)
    {
        File.AppendAllText(path, textToAppend + Environment.NewLine);
    }
    
    
}
