using DocumentFormat.OpenXml.Wordprocessing;
using Document = ConsoleWord.Core.Entities.Document;

namespace ConsoleWord.Core.Decorators
{
    public class BoldDecorator : TextDecorator
    {
        public BoldDecorator(Document doc) : base(doc) { }

        public override void InsertText(string text)
        {
            Run run = new Run(new Text(text));
            
            RunProperties runProperties = run.GetFirstChild<RunProperties>();
            if (runProperties == null)
            {
                runProperties = new RunProperties();
                run.PrependChild(runProperties);
            }
            runProperties.Append(new Bold());
            
            InsertBaseText(run);
        }
    }
}