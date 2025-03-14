using ConsolePaint.Models;
using ConsolePaint.Interfaces;

namespace ConsolePaint.Services
{
    public class HistoryService: IHistoryService
    {
        private Stack<Dictionary<string, Shape>> historyStack = new();
        private Stack<Dictionary<string, Shape>> redoStack = new();

        public void SaveState(Dictionary<string, Shape> currentState)
        {
            historyStack.Push(new Dictionary<string, Shape>(currentState));
            redoStack.Clear(); 
        }

        public void Undo(ref Dictionary<string, Shape> shapes)
        {
            if (historyStack.Count > 0)
            { 
                redoStack.Push(new Dictionary<string, Shape>(shapes));
                shapes = historyStack.Pop();
            }
        }

        public void Redo(ref Dictionary<string, Shape> shapes)
        {
            if (redoStack.Count > 0)
            {
                historyStack.Push(new Dictionary<string, Shape>(shapes));
                shapes = redoStack.Pop();
            }
        }
    }

}