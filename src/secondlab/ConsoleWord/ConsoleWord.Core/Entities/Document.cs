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
        public string Name { get; }
        public StringBuilder Content { get; set; } // Changed to StringBuilder for better manipulation
        public string Font { get; }
        public int TextSize { get; }

        // Private field to hold the WordprocessingDocument
        private WordprocessingDocument wordDoc;

        public Document(string name, string content, string font, int textSize)
        {
            Name = name;
            Content = new StringBuilder(content); // Now supports modifications
            Font = font;
            TextSize = textSize;
        }

        public void InsertText(Run run)
        {
            // Add text to the document (assuming you handle the actual Open XML insertion)
            Body body = GetBody(); // Get the body of the document (now using GetBody method)
            Paragraph paragraph = new Paragraph();
            paragraph.Append(run);
            body.Append(paragraph);
        }

        public Body GetBody()
        {
            // Initialize wordDoc if it is null
            if (wordDoc == null)
            {
                // Initialize WordprocessingDocument here, e.g., load from a file
                // Note: You should replace "path_to_document.docx" with the actual file path or logic to load your document
                wordDoc = WordprocessingDocument.Open("path_to_document.docx", true); // Use the actual path
            }

            return wordDoc.MainDocumentPart.Document.Body;
        }

        // Method to delete text from the content
        public virtual void DeleteText(int startIndex, int length)
        {
            if (startIndex >= 0 && startIndex < Content.Length && length > 0)
            {
                Content.Remove(startIndex, Math.Min(length, Content.Length - startIndex));
            }
        }

        // Method to apply bold formatting
        public virtual void FormatBold()
        {
            Content.Insert(0, "**");
            Content.Append("**");
        }

        // Method to apply italic formatting
        public virtual void FormatItalic()
        {
            Content.Insert(0, "*");
            Content.Append("*");
        }

        // Method to apply underline formatting
        public virtual void FormatUnderline()
        {
            Content.Insert(0, "__");
            Content.Append("__");
        }

        public override string ToString() => Content.ToString();
    }
}
