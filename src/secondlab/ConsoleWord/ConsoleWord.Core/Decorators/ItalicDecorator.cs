using DocumentFormat.OpenXml.Wordprocessing;
using ConsoleWord.Core.Entities;
using Document = ConsoleWord.Core.Entities.Document;

namespace ConsoleWord.Core.Decorators
{
    public class ItalicDecorator : TextDecorator
    {
        public ItalicDecorator(Document doc) : base(doc) { }

        public override void InsertText(string text)
        {
            // Create a Run with the provided text
            Run run = new Run(new Text(text));

            // Apply italic to the RunProperties
            RunProperties runProperties = run.GetFirstChild<RunProperties>();
            if (runProperties == null)
            {
                runProperties = new RunProperties();
                run.PrependChild(runProperties);
            }
            runProperties.Append(new Italic());

            // Delegate the actual insertion of the run to the base Document
            InsertBaseText(run);
        }
    }
}