using ConsolePaint.Models;
using System.Collections.Generic;

namespace ConsolePaint.Services
{
    public class HistoryService
    {
        private Stack<Dictionary<string, Shape>> historyStack = new();
        private Stack<Dictionary<string, Shape>> redoStack = new();

        public void SaveState(Dictionary<string, Shape> currentState)
        {
            // Save a deep copy of the current state
            historyStack.Push(new Dictionary<string, Shape>(currentState));
            redoStack.Clear(); // Clear the redo stack when a new action is performed
        }

        public void Undo(ref Dictionary<string, Shape> shapes)
        {
            if (historyStack.Count > 0)
            {
                // Save the current state to the redo stack before undoing
                redoStack.Push(new Dictionary<string, Shape>(shapes));

                // Pop the last saved state
                shapes = historyStack.Pop();
            }
        }

        public void Redo(ref Dictionary<string, Shape> shapes)
        {
            if (redoStack.Count > 0)
            {
                // Save the current state to the history stack before redoing
                historyStack.Push(new Dictionary<string, Shape>(shapes));

                // Pop the last redo state
                shapes = redoStack.Pop();
            }
        }
    }

}