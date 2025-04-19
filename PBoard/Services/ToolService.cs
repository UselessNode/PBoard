using System.Windows;
using System.Windows.Input;
using PBoard.Tools;

namespace PBoard.Services
{
    /// <summary>
    /// Сервис для управления инструментами
    /// </summary>
    public class ToolService
    {
        private readonly Dictionary<string, ITool> tools = new Dictionary<string, ITool>();
        private ITool? activeTool;
        
        /// <summary>
        /// Регистрирует инструмент в сервисе
        /// </summary>
        public void RegisterTool(ITool tool)
        {
            tools[tool.Name] = tool;
        }
        
        /// <summary>
        /// Активирует инструмент по имени
        /// </summary>
        public void ActivateTool(string toolName)
        {
            if (tools.TryGetValue(toolName, out var tool))
            {
                // Деактивируем текущий инструмент
                activeTool?.Deactivate();
                
                // Активируем новый инструмент
                tool.Activate();
                activeTool = tool;
            }
        }
        
        /// <summary>
        /// Возвращает текущий активный инструмент
        /// </summary>
        public ITool? GetActiveTool()
        {
            return activeTool;
        }
        
        /// <summary>
        /// Обработка события нажатия кнопки мыши
        /// </summary>
        public void OnMouseDown(Point position, MouseButtonEventArgs e)
        {
            activeTool?.OnMouseDown(position, e);
        }
        
        /// <summary>
        /// Обработка события перемещения мыши
        /// </summary>
        public void OnMouseMove(Point position, MouseEventArgs e)
        {
            activeTool?.OnMouseMove(position, e);
        }
        
        /// <summary>
        /// Обработка события отпускания кнопки мыши
        /// </summary>
        public void OnMouseUp(Point position, MouseButtonEventArgs e)
        {
            activeTool?.OnMouseUp(position, e);
        }
    }
} 