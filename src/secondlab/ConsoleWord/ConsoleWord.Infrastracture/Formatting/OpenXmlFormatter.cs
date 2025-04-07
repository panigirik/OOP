using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ConsoleWord.Infrastracture.Formatting;

public class OpenXmlFormatter
{
    public void ApplyTextDecorations(string filePath, bool isBold, bool isItalic, bool isUnderline)
    {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, true))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;
            foreach (var paragraph in body.Elements<Paragraph>())
            {
                foreach (var run in paragraph.Elements<Run>())
                {
                    RunProperties props = run.RunProperties ?? new RunProperties();

                    if (isBold)
                        props.Bold = new Bold();
                    if (isItalic)
                        props.Italic = new Italic();
                    if (isUnderline)
                        props.Underline = new Underline() { Val = UnderlineValues.Single };

                    run.RunProperties = props;
                }
            }
            wordDoc.MainDocumentPart.Document.Save();
        }
    }
}