using System.Windows;
using System.Windows.Input;
using PBoard.Models;
using PBoard.Services;

namespace PBoard.Tools
{
    /// <summary>
    /// Инструмент для создания расширенных текстовых блоков с форматированием
    /// </summary>
    public class RichTextTool : ITool
    {
        private readonly BoardService boardService;
        private RichTextItem? activeTextItem;
        
        public string Name => "Форматированный текст";
        
        public RichTextTool(BoardService boardService)
        {
            this.boardService = boardService;
        }
        
        public void Activate()
        {
            // Дополнительная инициализация при активации инструмента
        }
        
        public void Deactivate()
        {
            // Финализируем редактирование текста при деактивации инструмента
            if (activeTextItem != null)
            {
                FinalizeTextEditing();
            }
        }
        
        public void OnMouseDown(Point position, MouseButtonEventArgs? e)
        {
            if (e?.ChangedButton == MouseButton.Left && e.Source is UIElement)
            {
                // Создаем новый текстовый блок
                CreateTextItem(position);
                e.Handled = true;
            }
        }
        
        public void OnMouseMove(Point position, MouseEventArgs? e)
        {
            // Этот инструмент не требует обработки движения мыши
        }
        
        public void OnMouseUp(Point position, MouseButtonEventArgs? e)
        {
            // Этот инструмент не требует обработки отпускания кнопки мыши
        }
        
        /// <summary>
        /// Создание нового расширенного текстового элемента
        /// </summary>
        private void CreateTextItem(Point position)
        {
            // Финализируем предыдущий текстовый элемент, если он есть
            if (activeTextItem != null)
            {
                FinalizeTextEditing();
            }
            
            try
            {
                // Создаем новый расширенный текстовый элемент с пустым текстом
                // Текст будет установлен при вызове StartEditing
                activeTextItem = new RichTextItem("");
                
                // Добавляем элемент на доску
                //boardService.AddItem(activeTextItem, position);
                boardService.AddItemWithHistory(activeTextItem, position);
                
                // Начинаем редактирование после добавления на доску
                activeTextItem.StartEditing();
                
                // Обработка события завершения редактирования (будет вызвано при потере фокуса)
                // или при деактивации инструмента
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Ошибка при создании текстового элемента: " + ex.Message);
                activeTextItem = null;
            }
        }
        
        /// <summary>
        /// Завершение редактирования текстового элемента
        /// </summary>
        private void FinalizeTextEditing()
        {
            if (activeTextItem != null)
            {
                if (string.IsNullOrWhiteSpace(activeTextItem.Text))
                {
                    // Если текст пустой, удаляем текстовое поле через систему истории
                    boardService.RemoveItemWithHistory(activeTextItem);
                }
                else
                {
                    // Завершаем редактирование
                    activeTextItem.FinishEditing();
                }
                
                activeTextItem = null;
            }
        }
    }
} 