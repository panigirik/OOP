
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

        // Используем метод из DocumentEditor для чтения содержимого документа
        string documentText = ReadDocxContent(filePath);

        // Получаем имя документа
        string documentName = Path.GetFileNameWithoutExtension(filePath);

        // Возвращаем объект Document с полученным содержимым
        var document = new Document(documentName, documentText, "Arial", 12); // Можно использовать другие параметры
        return document;
    }

    public void RemoveLastAppendedText(string path, int textLength)
    {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;
            var lastParagraph = body.Elements<Paragraph>().LastOrDefault();

            if (lastParagraph != null)
            {
                var run = lastParagraph.Elements<Run>().LastOrDefault();
                var textElement = run?.Elements<Text>().LastOrDefault();

                if (textElement != null && !string.IsNullOrEmpty(textElement.Text))
                {
                    if (textElement.Text.Length > textLength)
                    {
                        textElement.Text = textElement.Text[..^textLength];
                    }
                    else
                    {
                        lastParagraph.Remove(); // удаляем весь параграф, если весь текст — это добавленное
                    }
                }

                wordDoc.MainDocumentPart.Document.Save();
            }
        }
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
            body.RemoveAllChildren(); // удаляем всё старое

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

    
    public void ClearTextFileContent(string path)
    {
        File.WriteAllText(path, string.Empty);
    }
    
}
