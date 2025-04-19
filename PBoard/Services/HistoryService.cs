using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PBoard.Services
{
    /// <summary>
    /// Интерфейс команды для системы отмены/возврата действий
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Выполняет команду
        /// </summary>
        void Execute();
        
        /// <summary>
        /// Отменяет команду
        /// </summary>
        void Undo();
        
        /// <summary>
        /// Краткое описание команды для отображения в интерфейсе
        /// </summary>
        string Description { get; }
    }
    
    /// <summary>
    /// Сервис для управления историей действий и обеспечения отмены/возврата
    /// </summary>
    public class HistoryService
    {
        private readonly Stack<ICommand> undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> redoStack = new Stack<ICommand>();
        private int maxStackSize = 50; // Максимальное количество команд в истории
        
        /// <summary>
        /// Событие, вызываемое при изменении состояния истории
        /// </summary>
        public event EventHandler? HistoryChanged;
        
        /// <summary>
        /// Указывает, можно ли отменить последнее действие
        /// </summary>
        public bool CanUndo => undoStack.Count > 0;
        
        /// <summary>
        /// Указывает, можно ли вернуть отмененное действие
        /// </summary>
        public bool CanRedo => redoStack.Count > 0;
        
        /// <summary>
        /// Возвращает описание последней команды для отмены
        /// </summary>
        public string UndoDescription => CanUndo ? undoStack.Peek().Description : string.Empty;
        
        /// <summary>
        /// Возвращает описание последней команды для возврата
        /// </summary>
        public string RedoDescription => CanRedo ? redoStack.Peek().Description : string.Empty;
        
        /// <summary>
        /// Выполняет команду и добавляет её в историю
        /// </summary>
        public void ExecuteCommand(ICommand command)
        {
            try
            {
                // Выполняем команду
                command.Execute();
                
                // Добавляем команду в стек отмены
                undoStack.Push(command);
                
                // Очищаем стек возврата, так как была выполнена новая команда
                redoStack.Clear();
                
                // Если стек превысил максимальный размер, удаляем самые старые команды
                if (undoStack.Count > maxStackSize)
                {
                    // Создаем новый стек с последними N командами
                    var tempStack = new Stack<ICommand>(undoStack.Take(maxStackSize).Reverse());
                    undoStack.Clear();
                    
                    // Возвращаем команды в основной стек
                    foreach (var cmd in tempStack)
                    {
                        undoStack.Push(cmd);
                    }
                }
                
                // Уведомляем об изменении состояния истории
                OnHistoryChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении команды: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Отменяет последнюю команду
        /// </summary>
        public void Undo()
        {
            if (!CanUndo)
                return;
                
            try
            {
                // Получаем последнюю команду из стека отмены
                var command = undoStack.Pop();
                
                // Отменяем команду
                command.Undo();
                
                // Добавляем команду в стек возврата
                redoStack.Push(command);
                
                // Уведомляем об изменении состояния истории
                OnHistoryChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене действия: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Возвращает последнюю отмененную команду
        /// </summary>
        public void Redo()
        {
            if (!CanRedo)
                return;
                
            try
            {
                // Получаем команду из стека возврата
                var command = redoStack.Pop();
                
                // Выполняем команду снова
                command.Execute();
                
                // Добавляем команду обратно в стек отмены
                undoStack.Push(command);
                
                // Уведомляем об изменении состояния истории
                OnHistoryChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при возврате действия: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Очищает всю историю действий
        /// </summary>
        public void ClearHistory()
        {
            undoStack.Clear();
            redoStack.Clear();
            
            OnHistoryChanged();
        }
        
        /// <summary>
        /// Вызывает событие изменения истории
        /// </summary>
        protected virtual void OnHistoryChanged()
        {
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
} 