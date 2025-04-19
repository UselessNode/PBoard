using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using PBoard.Models;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace PBoard.Services
{
    /// <summary>
    /// Сервис для работы с проектами (сохранение, загрузка)
    /// </summary>
    public class ProjectService
    {
        private readonly BoardService boardService;
        
        /// <summary>
        /// Класс для хранения данных проекта
        /// </summary>
        [Serializable]
        public class ProjectData
        {
            /// <summary>
            /// Версия формата
            /// </summary>
            public string Version { get; set; } = "1.0";
            
            /// <summary>
            /// Дата создания файла
            /// </summary>
            public DateTime CreationDate { get; set; } = DateTime.Now;
            
            /// <summary>
            /// Список элементов
            /// </summary>
            public List<BoardItemData> Items { get; set; } = new List<BoardItemData>();
        }
        
        /// <summary>
        /// Класс для хранения данных элемента
        /// </summary>
        [Serializable]
        public class BoardItemData
        {
            /// <summary>
            /// Тип элемента
            /// </summary>
            public string Type { get; set; } = "";
            
            /// <summary>
            /// Позиция X
            /// </summary>
            public double X { get; set; }
            
            /// <summary>
            /// Позиция Y
            /// </summary>
            public double Y { get; set; }
            
            /// <summary>
            /// Ширина
            /// </summary>
            public double Width { get; set; }
            
            /// <summary>
            /// Высота
            /// </summary>
            public double Height { get; set; }
            
            /// <summary>
            /// Данные содержимого в зависимости от типа
            /// </summary>
            public string Content { get; set; } = "";
            
            /// <summary>
            /// Дополнительные данные (для форматированного текста - XAML)
            /// </summary>
            public string ExtraData { get; set; } = "";
        }
        
        public ProjectService(BoardService boardService)
        {
            this.boardService = boardService;
        }
        
        /// <summary>
        /// Экспортирует проект в файл
        /// </summary>
        public bool ExportProject(string filePath)
        {
            try
            {
                // Создаем объект для хранения данных проекта
                ProjectData projectData = new ProjectData();
                
                // Получаем все элементы с доски
                var items = boardService.GetAllItems();
                
                // Сериализуем каждый элемент
                foreach (var item in items)
                {
                    BoardItemData itemData = new BoardItemData
                    {
                        X = item.Left,
                        Y = item.Top,
                        Width = item.Width,
                        Height = item.Height
                    };
                    
                    // Определяем тип элемента и сохраняем соответствующие данные
                    if (item is RichTextItem textItem)
                    {
                        itemData.Type = "RichText";
                        itemData.Content = textItem.Text;
                        itemData.ExtraData = textItem.FormattedContent;
                    }
                    else if (item is ImageItem imageItem && imageItem.Source != null)
                    {
                        itemData.Type = "Image";
                        
                        // Преобразуем изображение в Base64-строку
                        string base64Image = ConvertImageToBase64(imageItem.Source);
                        itemData.Content = base64Image;
                        
                        // Можем сохранить дополнительные параметры изображения в ExtraData если необходимо
                        // itemData.ExtraData = "...";
                    }
                    
                    projectData.Items.Add(itemData);
                }
                
                // Сериализуем проект в XML
                XmlSerializer serializer = new XmlSerializer(typeof(ProjectData));
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(fs, projectData);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте проекта: {ex.Message}", "Ошибка экспорта",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        
        /// <summary>
        /// Преобразует изображение в строку Base64
        /// </summary>
        private string ConvertImageToBase64(BitmapImage bitmapImage)
        {
            string base64 = string.Empty;
            
            try
            {
                // Конвертируем BitmapImage в PNG через PngBitmapEncoder
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    byte[] imageBytes = ms.ToArray();
                    base64 = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при кодировании изображения: {ex.Message}", 
                    "Ошибка экспорта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            return base64;
        }
        
        /// <summary>
        /// Преобразует строку Base64 в изображение BitmapImage
        /// </summary>
        private BitmapImage? ConvertBase64ToImage(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return null;
                
            try
            {
                // Декодируем строку в массив байтов
                byte[] imageBytes = Convert.FromBase64String(base64);
                
                // Создаем изображение из массива байтов
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); // Чтобы изображение можно было использовать в разных потоках
                }
                
                return bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при декодировании изображения: {ex.Message}", 
                    "Ошибка импорта", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        
        /// <summary>
        /// Импортирует проект из файла
        /// </summary>
        public bool ImportProject(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    MessageBox.Show("Указанный файл не существует", "Ошибка импорта",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                // Очищаем доску перед импортом
                boardService.ClearAll();
                
                // Десериализуем проект из XML
                XmlSerializer serializer = new XmlSerializer(typeof(ProjectData));
                ProjectData? projectData = null;
                
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    object? result = serializer.Deserialize(fs);
                    if (result is ProjectData data)
                    {
                        projectData = data;
                    }
                }
                
                // Проверяем, что проект успешно загружен
                if (projectData == null || projectData.Items == null)
                {
                    MessageBox.Show("Не удалось загрузить данные проекта", "Ошибка импорта",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                // Восстанавливаем каждый элемент
                foreach (var itemData in projectData.Items)
                {
                    BoardItem? item = null;
                    
                    // Создаем элемент в зависимости от типа
                    if (itemData.Type == "RichText")
                    {
                        RichTextItem textItem = new RichTextItem();
                        textItem.Text = itemData.Content ?? string.Empty;
                        
                        // Восстанавливаем форматированное содержимое, если оно есть
                        if (!string.IsNullOrEmpty(itemData.ExtraData))
                        {
                            textItem.FormattedContent = itemData.ExtraData;
                        }
                        
                        item = textItem;
                    }
                    else if (itemData.Type == "Image")
                    {
                        // Восстанавливаем изображение из Base64-строки
                        if (!string.IsNullOrEmpty(itemData.Content))
                        {
                            BitmapImage? bitmap = ConvertBase64ToImage(itemData.Content);
                            
                            if (bitmap != null)
                            {
                                ImageItem imageItem = new ImageItem(bitmap);
                                item = imageItem;
                            }
                            else
                            {
                                MessageBox.Show("Не удалось декодировать изображение",
                                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Изображение не содержит данных",
                                "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    
                    // Если элемент был создан, добавляем его на доску
                    if (item != null)
                    {
                        // Добавляем элемент на доску
                        boardService.AddItem(item, new Point(itemData.X, itemData.Y));
                        
                        // Устанавливаем размеры
                        item.SetSize(itemData.Width, itemData.Height);
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте проекта: {ex.Message}", "Ошибка импорта",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
} 