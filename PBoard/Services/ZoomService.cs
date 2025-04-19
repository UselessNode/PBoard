using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PBoard.Services
{
    /// <summary>
    /// Сервис для управления масштабированием и панорамированием
    /// </summary>
    public class ZoomService
    {
        private readonly Canvas workArea;
        private readonly ScrollViewer scrollViewer;
        private ScaleTransform scaleTransform;
        private TranslateTransform translateTransform;
        private Point lastPanPoint;
        private bool isPanning;
        
        public double CurrentZoom => scaleTransform.ScaleX;
        
        public ZoomService(Canvas workArea, ScrollViewer scrollViewer)
        {
            this.workArea = workArea;
            this.scrollViewer = scrollViewer;
            
            // Настройка трансформаций
            scaleTransform = new ScaleTransform(1, 1);
            translateTransform = new TranslateTransform(0, 0);
            
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            workArea.RenderTransform = transformGroup;
        }
        
        /// <summary>
        /// Обработка колеса мыши для масштабирования
        /// </summary>
        public void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Получаем текущую позицию мыши относительно холста
                Point mousePos = e.GetPosition(workArea);
                
                // Получаем позицию до масштабирования относительно углов холста
                double offsetX = mousePos.X - translateTransform.X;
                double offsetY = mousePos.Y - translateTransform.Y;
                
                // Изменяем масштаб
                double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
                
                // Ограничиваем масштаб
                double oldScale = scaleTransform.ScaleX;
                double newScale = oldScale * zoomFactor;
                if (newScale < 0.1) newScale = 0.1;
                if (newScale > 5.0) newScale = 5.0;
                
                // Применяем масштабирование
                scaleTransform.ScaleX = newScale;
                scaleTransform.ScaleY = newScale;
                
                // Корректируем смещение, чтобы точка под курсором осталась на том же месте
                double zoomChange = newScale / oldScale;
                double newOffsetX = offsetX * zoomChange;
                double newOffsetY = offsetY * zoomChange;
                
                // Вычисляем корректирующий сдвиг
                double adjustX = newOffsetX - offsetX;
                double adjustY = newOffsetY - offsetY;
                
                // Перемещаем видимую область, чтобы удерживать курсор на той же позиции
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + adjustX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + adjustY);
                
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Начало панорамирования при нажатии средней кнопки мыши
        /// </summary>
        public void StartPanning(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                // Начало перемещения вида
                isPanning = true;
                lastPanPoint = e.GetPosition(scrollViewer);
                scrollViewer.Cursor = Cursors.Hand;
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Панорамирование при перемещении мыши
        /// </summary>
        public void DoPanning(MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && isPanning)
            {
                Point currentPosition = e.GetPosition(scrollViewer);
                
                // Вычисляем смещение
                double deltaX = currentPosition.X - lastPanPoint.X;
                double deltaY = currentPosition.Y - lastPanPoint.Y;
                
                // Обновляем положение прокрутки
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - deltaX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - deltaY);
                
                // Обновляем начальную точку для следующего перемещения
                lastPanPoint = currentPosition;
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Завершение панорамирования при отпускании кнопки мыши
        /// </summary>
        public void StopPanning(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                isPanning = false;
                scrollViewer.Cursor = Cursors.Arrow;
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Установка конкретного уровня масштабирования
        /// </summary>
        public void SetZoom(double zoomLevel)
        {
            // Ограничиваем масштаб
            if (zoomLevel < 0.1) zoomLevel = 0.1;
            if (zoomLevel > 5.0) zoomLevel = 5.0;
            
            // Применяем масштабирование
            scaleTransform.ScaleX = zoomLevel;
            scaleTransform.ScaleY = zoomLevel;
        }
        
        /// <summary>
        /// Преобразует точку из координат Canvas в координаты экрана с учетом масштаба
        /// </summary>
        public Point ConvertCanvasPointToScreen(Point canvasPoint)
        {
            // Учитываем масштаб и смещение
            return new Point(
                canvasPoint.X * scaleTransform.ScaleX + translateTransform.X,
                canvasPoint.Y * scaleTransform.ScaleY + translateTransform.Y
            );
        }
        
        /// <summary>
        /// Преобразует точку из координат экрана в координаты Canvas с учетом масштаба
        /// </summary>
        public Point ConvertScreenPointToCanvas(Point screenPoint)
        {
            // Учитываем масштаб и смещение
            return new Point(
                (screenPoint.X - translateTransform.X) / scaleTransform.ScaleX,
                (screenPoint.Y - translateTransform.Y) / scaleTransform.ScaleY
            );
        }
    }
} 