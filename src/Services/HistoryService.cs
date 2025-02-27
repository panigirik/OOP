using ConsolePaint.Interfaces;
using ConsolePaint.Models;

namespace ConsolePaint.Services
{
    public class HistoryService: IHistoryService
    {
        private Stack<char[,]> history = new Stack<char[,]>();
        private Stack<char[,]> redoStack = new Stack<char[,]>();

        public void SaveState(char[,] currentState)
        {
            // Сохранение текущего состояния канвы
            char[,] stateCopy = (char[,])currentState.Clone();  // Глубокое копирование
            history.Push(stateCopy);
            redoStack.Clear();  // Очистка стека redo, если после изменений не было новых операций
        }

        public void Undo(ref char[,] canvas)
        {
            if (history.Count > 0)
            {
                redoStack.Push(canvas);  // Сохранение текущего состояния в redo-стек перед отменой
                canvas = history.Pop();  // Восстановление предыдущего состояния канвы
            }
            else
            {
                Console.WriteLine("Нет доступных операций для отмены.");
            }
        }

        public void Redo(ref char[,] canvas)
        {
            if (redoStack.Count > 0)
            {
                history.Push(canvas);  // Сохранение текущего состояния в историю перед повтором
                canvas = redoStack.Pop();  // Восстановление состояния из redo-стека
            }
            else
            {
                Console.WriteLine("Нет доступных операций для повторения.");
            }
        }
    }

}