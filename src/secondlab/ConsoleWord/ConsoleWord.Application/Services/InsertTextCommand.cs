using ConsoleWord.Application.Interfaces;
using ConsoleWord.Core.Entities;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = ConsoleWord.Core.Entities.Document;

namespace ConsoleWord.Application.Services
{
    public class InsertTextCommand : ICommand
    {
        private Document _document;
        private string _text;

        public InsertTextCommand(Document doc, string text)
        {
            _document = doc;
            _text = text;
        }

        public void Execute()
        {
            // Convert the string to a Run object
            Run run = new Run(new Text(_text));
            _document.InsertText(run);  // Pass the Run object to InsertText method
        }

        public void Undo()
        {
            // Calculate the starting index of the last inserted text and remove it
            _document.DeleteText(_document.ToString().Length - _text.Length, _text.Length);
        }
    }
}