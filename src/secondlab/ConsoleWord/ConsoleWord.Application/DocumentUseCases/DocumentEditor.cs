using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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
}
