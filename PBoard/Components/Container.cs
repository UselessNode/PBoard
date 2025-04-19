using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PBoard.Components
{
    /// <summary>
    /// Базовый класс для контейнеров элементов
    /// </summary>
    public class Container
    {
        #region Свойства

        // Флаги состояния
        public bool IsSelected { get; set; }
        public bool IsEditing { get; set; }

        // Заголовок контейнера
        public string Title { get; set; } = "Контейнер";
        
        // Позиция контейнера
        public double X { get; set; }
        public double Y { get; set; }

        // Размеры контейнера
        public double Width { get; set; } = 200;
        public double Height { get; set; } = 100;

        #endregion

        #region Конструктор

        public Container()
        {
            // Базовая инициализация
        }

        #endregion
    }
} 