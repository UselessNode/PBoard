using System.Windows;
using System.Windows.Input;

namespace PBoard.Tools
{
    /// <summary>
    /// Интерфейс для инструментов доски
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// Название инструмента
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Обработка события нажатия кнопки мыши
        /// </summary>
        void OnMouseDown(Point position, MouseButtonEventArgs? e);
        
        /// <summary>
        /// Обработка события перемещения мыши
        /// </summary>
        void OnMouseMove(Point position, MouseEventArgs? e);
        
        /// <summary>
        /// Обработка события отпускания кнопки мыши
        /// </summary>
        void OnMouseUp(Point position, MouseButtonEventArgs? e);
        
        /// <summary>
        /// Активация инструмента
        /// </summary>
        void Activate();
        
        /// <summary>
        /// Деактивация инструмента
        /// </summary>
        void Deactivate();
    }
} 