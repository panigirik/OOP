using System;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;

namespace ConsoleWord.Core.Entities
{
    public class Document
    {
        public string Name { get; set; }
        public StringBuilder Content { get; set; }
        public string Font { get; set; }
        public int TextSize { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }

        
        private WordprocessingDocument wordDoc;

        public Document(string name, string content, string font, int textSize)
        {
            Name = name;
            Content = new StringBuilder(content);
            Font = font;
            TextSize = textSize;
        }

        public Document() { }
        
        public void InsertText(Run run)
        {
            Body body = GetBody(); 
            Paragraph paragraph = new Paragraph();
            paragraph.Append(run);
            body.Append(paragraph);
        }

        public Body GetBody()
        {
            if (wordDoc == null)
            {
                wordDoc = WordprocessingDocument.Open("path_to_document.docx", true); 
            }

            return wordDoc.MainDocumentPart.Document.Body;
        }
        
        public virtual void DeleteText(int startIndex, int length)
        {
            if (startIndex >= 0 && startIndex < Content.Length && length > 0)
            {
                Content.Remove(startIndex, Math.Min(length, Content.Length - startIndex));
            }
        }
        
        public virtual void FormatBold()
        {
            Content.Insert(0, "**");
            Content.Append("**");
        }
        
        public virtual void FormatItalic()
        {
            Content.Insert(0, "*");
            Content.Append("*");
        }
        
        public virtual void FormatUnderline()
        {
            Content.Insert(0, "__");
            Content.Append("__");
        }

        public override string ToString() => Content.ToString();
    }
}
