using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Interfaces;

namespace ConsoleWord.Application.Commands
{
    public class AppendTextCommand : IDocumentCommand
    {
        private readonly string _path;
        private readonly string _textToAppend;
        private readonly DocumentEditor _editor;
        private readonly bool _isTextFile;
        private string? _previousContent;

        public AppendTextCommand(string path, string textToAppend, DocumentEditor editor, bool isTextFile = false)
        {
            _path = path;
            _textToAppend = textToAppend;
            _editor = editor;
            _isTextFile = isTextFile;
        }

        public void Execute()
        {
            _previousContent = _isTextFile
                ? _editor.ReadTextFileContent(_path)
                : _editor.ReadDocxContent(_path);

            if (_isTextFile)
                _editor.AppendTextToTextFile(_path, _textToAppend);
            else
                _editor.AppendTextToDocx(_path, _textToAppend);
        }

        public void Undo()
        {
            if (_previousContent is not null)
            {
                if (_isTextFile)
                    _editor.OverwriteTextFile(_path, _previousContent);
                else
                    _editor.OverwriteDocxFile(_path, _previousContent);
            }
        }
    }
}