using DocumentFormat.OpenXml.Wordprocessing;
using ConsoleWord.Core.Entities;
using Document = ConsoleWord.Core.Entities.Document;

namespace ConsoleWord.Core.Decorators
{
    public class BoldDecorator : TextDecorator
    {
        public BoldDecorator(Document doc) : base(doc) { }

        public override void InsertText(string text)
        {
            // Create a new Run object for the text
            Run run = new Run(new Text(text));

            // Apply bold to the RunProperties
            RunProperties runProperties = run.GetFirstChild<RunProperties>();
            if (runProperties == null)
            {
                runProperties = new RunProperties();
                run.PrependChild(runProperties);
            }
            runProperties.Append(new Bold());

            // Delegate the actual insertion of the run to the base Document
            InsertBaseText(run);
        }
    }
}