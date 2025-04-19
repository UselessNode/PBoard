using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PBoard.Models
{
    /// <summary>
    /// Представляет элемент изображения на доске
    /// </summary>
    public class ImageItem : BoardItem
    {
        // Путь к файлу изображения
        public string FilePath { get; set; } = string.Empty;
        
        // Имя файла изображения
        public string FileName { get; set; } = string.Empty;
        
        public BitmapImage? Source
        {
            get => ImageElement?.Source as BitmapImage;
            set
            {
                if (ImageElement != null)
                    ImageElement.Source = value;
            }
        }
        
        public Image? ImageElement => Element as Image;
        
        public ImageItem(BitmapImage source)
        {
            // Создаем элемент изображения
            Image image = new Image
            {
                Source = source,
                Width = source.Width,
                Height = source.Height,
                Stretch = Stretch.Uniform
            };
            
            Element = image;
            
            // Устанавливаем минимальный размер (если изображение слишком маленькое)
            if (source.Width < 100 || source.Height < 100)
            {
                double scale = Math.Max(100 / source.Width, 100 / source.Height);
                Width = source.Width * scale;
                Height = source.Height * scale;
            }
        }
        
        /// <summary>
        /// Создает манипуляторы изменения размера для изображения
        /// </summary>
        public override void CreateResizeThumbs(Canvas canvas)
        {
            // Вызываем базовую реализацию для создания манипуляторов
            base.CreateResizeThumbs(canvas);
            
            // Дополнительная логика для изображений при необходимости
        }
        
        public override BoardItem Clone()
        {
            if (Source == null)
            {
                throw new InvalidOperationException("Невозможно клонировать изображение без источника");
            }
            
            ImageItem clone = new ImageItem(Source)
            {
                FilePath = this.FilePath,
                FileName = this.FileName
            };
            
            clone.SetPosition(this.Left + 20, this.Top + 20); // Смещаем копию
            return clone;
        }
    }
} 