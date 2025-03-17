using DocumentFormat.OpenXml.Wordprocessing;
using ConsoleWord.Core.Entities;
using Document = ConsoleWord.Core.Entities.Document;

namespace ConsoleWord.Core.Decorators
{
    public abstract class TextDecorator : Document
    {
        protected Document _document;

        public TextDecorator(Document doc) : base(doc.Name, doc.Content.ToString(), doc.Font, doc.TextSize)
        {
            _document = doc;
        }

        public abstract void InsertText(string text);

        // Allow InsertText from the base document if needed
        public virtual void InsertBaseText(Run run)
        {
            _document.InsertText(run);  // Delegate to the base Document class
        }
    }
}