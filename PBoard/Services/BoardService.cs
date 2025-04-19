using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PBoard.Models;
using System.Windows.Media;
using System.Diagnostics;
using PBoard.Commands;

namespace PBoard.Services
{
    /// <summary>
    /// Сервис для управления элементами на доске
    /// </summary>
    public class BoardService
    {
        private readonly Canvas workArea;
        private ObservableCollection<BoardItem> items = new();
        private BoardItem? selectedItem;
        private HashSet<BoardItem> selectedItems = new();
        private HistoryService? historyService;
        
        /// <summary>
        /// Событие, вызываемое при изменении выделения
        /// </summary>
        public event EventHandler? SelectionChanged;
        
        public BoardService(Canvas workArea)
        {
            this.workArea = workArea;
        }
        
        /// <summary>
        /// Устанавливает сервис истории для отслеживания действий
        /// </summary>
        public void SetHistoryService(HistoryService historyService)
        {
            this.historyService = historyService;
        }
        
        /// <summary>
        /// Добавляет элемент на доску и регистрирует команду в истории
        /// </summary>
        public void AddItemWithHistory(BoardItem item, Point position)
        {
            if (historyService != null)
            {
                // Создаем команду добавления и выполняем через сервис истории
                var command = new AddItemCommand(this, item, position);
                historyService.ExecuteCommand(command);
            }
            else
            {
                // Если сервис истории не установлен, просто добавляем элемент
                AddItem(item, position);
            }
        }
        
        /// <summary>
        /// Удаляет элемент с доски и регистрирует команду в истории
        /// </summary>
        public void RemoveItemWithHistory(BoardItem item)
        {
            if (historyService != null)
            {
                // Создаем команду удаления и выполняем через сервис истории
                var command = new RemoveItemCommand(this, item);
                historyService.ExecuteCommand(command);
            }
            else
            {
                // Если сервис истории не установлен, просто удаляем элемент
                RemoveItem(item);
            }
        }
        
        /// <summary>
        /// Перемещает элемент и регистрирует команду в истории
        /// </summary>
        public void MoveItemWithHistory(BoardItem item, double originalX, double originalY, double newX, double newY)
        {
            if (historyService != null)
            {
                // Создаем команду перемещения и выполняем через сервис истории
                var command = new MoveItemCommand(item, originalX, originalY, newX, newY);
                historyService.ExecuteCommand(command);
            }
            else
            {
                // Если сервис истории не установлен, просто перемещаем элемент
                item.SetPosition(newX, newY);
            }
        }
        
        /// <summary>
        /// Изменяет размер элемента и регистрирует команду в истории
        /// </summary>
        public void ResizeItemWithHistory(BoardItem item, double originalWidth, double originalHeight, double newWidth, double newHeight)
        {
            if (historyService != null)
            {
                // Создаем команду изменения размера и выполняем через сервис истории
                var command = new ResizeItemCommand(item, originalWidth, originalHeight, newWidth, newHeight);
                historyService.ExecuteCommand(command);
            }
            else
            {
                // Если сервис истории не установлен, просто изменяем размер
                item.SetSize(newWidth, newHeight);
            }
        }
        
        /// <summary>
        /// Добавляет элемент на доску
        /// </summary>
        public void AddItem(BoardItem item, Point position)
        {
            // Установка позиции элемента
            item.SetPosition(position.X, position.Y);
            
            // Добавление в коллекцию и на Canvas
            items.Add(item);
            workArea.Children.Add(item.Element);
            
            // Устанавливаем Z-индекс элемента
            Canvas.SetZIndex(item.Element, items.Count);
            
            // Создание манипуляторов изменения размера
            item.CreateResizeThumbs(workArea);
            
            // Добавление обработчиков событий для элемента
            if (item.Element is UIElement element)
            {
                element.MouseDown += Element_MouseDown;
                
                // Устанавливаем контекстное меню, только если элемент поддерживает его
                if (element is FrameworkElement frameworkElement)
                {
                    frameworkElement.ContextMenu = CreateContextMenu();
                    Debug.WriteLine(@$"{element} был создан с контекстовым меню");
                }
            }
        }
        
        /// <summary>
        /// Удаляет элемент с доски
        /// </summary>
        public void RemoveItem(BoardItem item)
        {
            if (item != null)
            {
                // Сначала снимаем выделение с элемента, если он выбран
                if (selectedItems.Contains(item))
                {
                    selectedItems.Remove(item);
                    item.Deselect();
                }
                
                if (selectedItem == item)
                {
                    selectedItem = null;
                }
                
                // Удаляем элемент из рабочей области
                workArea.Children.Remove(item.Element);
                items.Remove(item);
                
                // Уведомляем об изменении выделения, если элемент был выбран
                OnSelectionChanged();
            }
        }
        
        /// <summary>
        /// Выбирает элемент
        /// </summary>
        public void SelectItem(BoardItem item, bool addToSelection = false)
        {
            if (!addToSelection)
            {
                // Снимаем выделение со всех элементов
                ClearSelection();
            }
            
            // Выделяем новый элемент
            selectedItem = item;
            selectedItems.Add(item);
            item.Select();
            
            // Обновляем отображение элементов
            RefreshCanvas();
            
            OnSelectionChanged();
        }
        
        /// <summary>
        /// Очищает текущее выделение
        /// </summary>
        public void ClearSelection()
        {
            // Снимаем выделение со всех элементов
            foreach (var item in selectedItems)
            {
                item.Deselect();
            }
            
            selectedItems.Clear();
            selectedItem = null;
            
            OnSelectionChanged();
        }
        
        /// <summary>
        /// Вызывает событие изменения выделения
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Выделяет все элементы внутри указанной области
        /// </summary>
        public void SelectItemsInArea(Rect selectionRect, bool addToSelection = false)
        {
            if (!addToSelection)
            {
                ClearSelection();
            }
            
            foreach (var item in items)
            {
                // Получаем границы элемента
                double left = item.Left;
                double top = item.Top;
                double right = left + item.Width;
                double bottom = top + item.Height;
                
                // Создаем прямоугольник элемента
                Rect itemRect = new Rect(left, top, right - left, bottom - top);
                
                // Проверяем, пересекаются ли прямоугольники или находится ли элемент внутри области выделения
                if (selectionRect.Contains(itemRect) || selectionRect.IntersectsWith(itemRect))
                {
                    selectedItems.Add(item);
                    item.Select();
                    
                    // Устанавливаем один элемент как текущий выбранный (для перемещения)
                    if (selectedItem == null)
                    {
                        selectedItem = item;
                    }
                }
            }
            
            // Обновляем отображение элементов
            RefreshCanvas();
            
            OnSelectionChanged();
        }
        
        /// <summary>
        /// Перемещает все выделенные элементы
        /// </summary>
        public void MoveSelectedItems(double deltaX, double deltaY)
        {
            foreach (var item in selectedItems)
            {
                double newLeft = item.Left + deltaX;
                double newTop = item.Top + deltaY;
                item.SetPosition(newLeft, newTop);
            }
            
            // Обновляем отображение элементов после перемещения
            RefreshCanvas();
        }
        
        /// <summary>
        /// Получает текущий выбранный элемент
        /// </summary>
        public BoardItem? GetSelectedItem()
        {
            return selectedItem;
        }
        
        /// <summary>
        /// Получает все выделенные элементы
        /// </summary>
        public IReadOnlyCollection<BoardItem> GetSelectedItems()
        {
            return selectedItems;
        }
        
        /// <summary>
        /// Создает копию выбранного элемента
        /// </summary>
        public void CopySelectedItem()
        {
            if (selectedItem != null)
            {
                BoardItem clone = selectedItem.Clone();
                AddItem(clone, new Point(0, 0)); // Позиция уже устанавливается в Clone()
                SelectItem(clone);
            }
        }
        
        /// <summary>
        /// Создает копии всех выделенных элементов
        /// </summary>
        public void CopySelectedItems()
        {
            if (selectedItems.Count == 0) return;
            
            var itemsToCopy = selectedItems.ToList();
            ClearSelection();
            
            foreach (var item in itemsToCopy)
            {
                BoardItem clone = item.Clone();
                AddItem(clone, new Point(0, 0)); // Позиция уже устанавливается в Clone()
                SelectItem(clone, true);
            }
        }
        
        /// <summary>
        /// Удаляет все выделенные элементы
        /// </summary>
        public void DeleteSelectedItems()
        {
            if (selectedItems.Count == 0) return;
            
            var itemsToDelete = selectedItems.ToList();
            ClearSelection();
            
            foreach (var item in itemsToDelete)
            {
                RemoveItem(item);
            }
        }
        
        /// <summary>
        /// Создает контекстное меню для элементов
        /// </summary>
        private ContextMenu CreateContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            // Пункт "Копировать"
            MenuItem copyItem = new MenuItem() { Header = "Копировать" };
            copyItem.Click += (s, e) => CopySelectedItems();
            menu.Items.Add(copyItem);
            
            // Пункт "Удалить"
            MenuItem deleteItem = new MenuItem() { Header = "Удалить" };
            deleteItem.Click += (s, e) => DeleteSelectedItems();
            menu.Items.Add(deleteItem);
            
            // Если выбран текстовый контейнер
            if (selectedItem is RichTextItem)
            {
                MenuItem formatItem = new MenuItem() { Header = "Форматирование" };
                formatItem.Click += (s, e) => 
                {
                    if (selectedItem is RichTextItem richTextItem)
                    {
                        // Создаем TextStyleViewModel из RichTextItem
                        var styleViewModel = Models.TextStyleAdapter.CreateFromRichTextItem(richTextItem);
                        
                        // Получаем родительское окно через workArea
                        Window parentWindow = Window.GetWindow(workArea);
                        
                        // Открываем диалог с созданной моделью представления
                        if (Views.TextStyleDialog.ShowDialog(parentWindow, styleViewModel, out var resultStyle))
                        {
                            // Применяем результат к RichTextItem
                            Models.TextStyleAdapter.ApplyStyleToRichTextItem(resultStyle, richTextItem);
                        }
                    }
                };
                menu.Items.Add(formatItem);
            }

            return menu;
        }
        
        /// <summary>
        /// Обработчик события щелчка мыши по элементу
        /// </summary>
        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Находим элемент в коллекции
            if (sender is UIElement uiElement)
            {
                BoardItem? item = items.FirstOrDefault(i => i.Element == uiElement);
                
                if (item != null && e.ChangedButton == MouseButton.Left)
                {
                    // Добавляем к выделению при нажатом Ctrl
                    bool addToSelection = Keyboard.Modifiers == ModifierKeys.Control;
                    SelectItem(item, addToSelection);
                    e.Handled = true;
                }
            }
        }
        
        /// <summary>
        /// Обновляет отображение всех элементов на рабочей области
        /// </summary>
        public void RefreshCanvas()
        {
            // Обновляем отображение всех выделенных элементов
            foreach (var item in selectedItems)
            {
                item.UpdateThumbsPosition();
                
                // Обновляем Z-индекс элемента, чтобы он был выше остальных
                if (item.Element is UIElement element)
                {
                    Canvas.SetZIndex(element, items.Count + 1);
                }
            }
        }
        
        /// <summary>
        /// Получает все элементы доски
        /// </summary>
        public IReadOnlyCollection<BoardItem> GetAllItems()
        {
            return items.ToList().AsReadOnly();
        }
        
        /// <summary>
        /// Очищает доску, удаляя все элементы
        /// </summary>
        public void ClearAll()
        {
            // Снимаем выделение со всех элементов
            ClearSelection();
            
            // Создаем копию списка для безопасного удаления во время перебора
            var itemsToRemove = items.ToList();
            
            // Удаляем все элементы
            foreach (var item in itemsToRemove)
            {
                RemoveItem(item);
            }
            
            // Убеждаемся, что списки пусты
            items.Clear();
            selectedItems.Clear();
            selectedItem = null;
            
            // Уведомляем об изменении выделения
            OnSelectionChanged();
        }
    }
} 