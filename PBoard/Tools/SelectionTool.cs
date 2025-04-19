using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using PBoard.Services;
using System.Collections.Generic;
using PBoard.Models;

namespace PBoard.Tools
{
    /// <summary>
    /// Инструмент для выделения и перемещения элементов
    /// </summary>
    public class SelectionTool : ITool
    {
        private readonly BoardService boardService;
        private Point startPoint;
        private bool isDragging;
        private bool isAreaSelecting;
        private Rectangle? selectionRectangle;
        private UIElement? lastHitElement;
        private readonly Dictionary<BoardItem, Point> initialPositions = new Dictionary<BoardItem, Point>();
        
        public string Name => "Выделение";
        
        public SelectionTool(BoardService boardService)
        {
            this.boardService = boardService;
        }
        
        /// <summary>
        /// Устанавливает прямоугольник выделения, который будет использоваться для отображения области выделения
        /// </summary>
        public void SetSelectionRectangle(Rectangle rectangle)
        {
            selectionRectangle = rectangle;
        }
        
        public void Activate()
        {
            // Дополнительная инициализация при активации инструмента
        }
        
        public void Deactivate()
        {
            // Очистка при деактивации инструмента
            isDragging = false;
            isAreaSelecting = false;
            HideSelectionRectangle();
        }
        
        public void OnMouseDown(Point position, MouseButtonEventArgs? e)
        {
            if (e?.ChangedButton == MouseButton.Left)
            {
                startPoint = position;
                lastHitElement = e.OriginalSource as UIElement;
                
                // Если клик был на пустом месте (на Canvas), то начинаем выделение областью
                if (e.OriginalSource is Canvas || e.Source is Canvas)
                {
                    isAreaSelecting = true;
                    ShowSelectionRectangle(startPoint);
                    
                    // Если не нажат Ctrl, очищаем текущее выделение
                    if (Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        boardService.ClearSelection();
                    }
                }
                else
                {
                    // Если клик был на элементе, возможно начинаем перетаскивание
                    isDragging = true;
                    
                    // Сохраняем начальные позиции всех выбранных элементов
                    initialPositions.Clear();
                    foreach (var item in boardService.GetSelectedItems())
                    {
                        initialPositions[item] = new Point(item.Left, item.Top);
                    }
                }
                
                e.Handled = true;
            }
        }
        
        public void OnMouseMove(Point position, MouseEventArgs? e)
        {
            if (e?.LeftButton == MouseButtonState.Pressed)
            {
                if (isAreaSelecting)
                {
                    // Обновляем прямоугольник выделения
                    UpdateSelectionRectangle(startPoint, position);
                    e.Handled = true;
                }
                else if (isDragging && boardService.GetSelectedItems().Count > 0)
                {
                    // Вычисляем смещение
                    double deltaX = position.X - startPoint.X;
                    double deltaY = position.Y - startPoint.Y;
                    
                    // Перемещаем все выделенные элементы (временно, без истории)
                    boardService.MoveSelectedItems(deltaX, deltaY);
                    
                    // Обновляем начальную точку для следующего перемещения
                    startPoint = position;
                    e.Handled = true;
                }
            }
        }
        
        public void OnMouseUp(Point position, MouseButtonEventArgs? e)
        {
            if (e?.ChangedButton == MouseButton.Left)
            {
                if (isAreaSelecting)
                {
                    // Завершаем выделение областью и выбираем элементы внутри прямоугольника
                    Rect selectionArea = CalculateSelectionRect(startPoint, position);
                    boardService.SelectItemsInArea(selectionArea, Keyboard.Modifiers == ModifierKeys.Control);
                    HideSelectionRectangle();
                    isAreaSelecting = false;
                }
                else if (isDragging)
                {
                    // Завершаем перемещение с записью в историю, если были перемещения
                    FinalizeMoveWithHistory();
                }
                
                isDragging = false;
                lastHitElement = null;
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Показывает прямоугольник выделения
        /// </summary>
        private void ShowSelectionRectangle(Point startPoint)
        {
            if (selectionRectangle != null)
            {
                selectionRectangle.Visibility = Visibility.Visible;
                Canvas.SetLeft(selectionRectangle, startPoint.X);
                Canvas.SetTop(selectionRectangle, startPoint.Y);
                selectionRectangle.Width = 0;
                selectionRectangle.Height = 0;
            }
        }
        
        /// <summary>
        /// Обновляет размер и положение прямоугольника выделения
        /// </summary>
        private void UpdateSelectionRectangle(Point startPoint, Point currentPoint)
        {
            if (selectionRectangle != null)
            {
                double left = Math.Min(startPoint.X, currentPoint.X);
                double top = Math.Min(startPoint.Y, currentPoint.Y);
                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);
                
                Canvas.SetLeft(selectionRectangle, left);
                Canvas.SetTop(selectionRectangle, top);
                selectionRectangle.Width = width;
                selectionRectangle.Height = height;
            }
        }
        
        /// <summary>
        /// Скрывает прямоугольник выделения
        /// </summary>
        private void HideSelectionRectangle()
        {
            if (selectionRectangle != null)
            {
                selectionRectangle.Visibility = Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// Вычисляет прямоугольник выделения на основе начальной и текущей точек
        /// </summary>
        private Rect CalculateSelectionRect(Point startPoint, Point currentPoint)
        {
            double left = Math.Min(startPoint.X, currentPoint.X);
            double top = Math.Min(startPoint.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - startPoint.X);
            double height = Math.Abs(currentPoint.Y - startPoint.Y);
            
            return new Rect(left, top, width, height);
        }
        
        /// <summary>
        /// Завершает перемещение с записью в историю
        /// </summary>
        private void FinalizeMoveWithHistory()
        {
            var selectedItems = boardService.GetSelectedItems();
            
            // Проверяем, были ли изменения в позициях элементов
            foreach (var item in selectedItems)
            {
                if (initialPositions.TryGetValue(item, out var initialPos))
                {
                    double currentX = item.Left;
                    double currentY = item.Top;
                    
                    // Если позиция изменилась, записываем действие в историю
                    if (Math.Abs(initialPos.X - currentX) > 0.1 || Math.Abs(initialPos.Y - currentY) > 0.1)
                    {
                        boardService.MoveItemWithHistory(item, initialPos.X, initialPos.Y, currentX, currentY);
                    }
                }
            }
            
            // Очищаем начальные позиции
            initialPositions.Clear();
        }
    }
} 