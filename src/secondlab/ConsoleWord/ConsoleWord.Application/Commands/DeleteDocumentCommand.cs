using ConsoleWord.Application.DocumentUseCases;
using System;
using System.IO;

namespace ConsoleWord.Application.Commands
{
    public class DeleteDocumentCommand : IDocumentCommand
    {
        private readonly string _path;
        private string? _backupContent;
        private bool _fileExistedBefore;
        private string? _extension;

        public DeleteDocumentCommand(string path)
        {
            _path = path;
        }

        public void Execute()
        {
            if (!File.Exists(_path))
            {
                Console.WriteLine("[Delete] File not found. Nothing to delete.");
                _fileExistedBefore = false;
                return;
            }

            try
            {
                _backupContent = File.ReadAllText(_path); // сохраняем содержимое
                _extension = Path.GetExtension(_path);
                File.Delete(_path); // удаляем файл
                _fileExistedBefore = true;

                Console.WriteLine($"[Delete] File deleted: {_path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Delete] Error deleting file: {ex.Message}");
            }
        }

        public void Undo()
        {
            if (!_fileExistedBefore || string.IsNullOrEmpty(_backupContent))
            {
                Console.WriteLine("[Undo Delete] Nothing to restore.");
                return;
            }

            try
            {
                File.WriteAllText(_path, _backupContent); // восстанавливаем файл
                Console.WriteLine($"[Undo Delete] File restored: {_path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Undo Delete] Error restoring file: {ex.Message}");
            }
        }
    }
}