using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PBoard.Models;
using PBoard.Services;
using System.IO;

namespace PBoard.Tools
{
    /// <summary>
    /// Инструмент для добавления изображений
    /// </summary>
    public class ImageTool : ITool
    {
        private readonly BoardService boardService;
        
        public string Name => "Изображение";
        
        public ImageTool(BoardService boardService)
        {
            this.boardService = boardService;
        }
        
        public void Activate()
        {
            // Дополнительная инициализация при активации инструмента
        }
        
        public void Deactivate()
        {
            // Очистка при деактивации инструмента
        }
        
        public void OnMouseDown(Point position, MouseButtonEventArgs? e)
        {
            if (e == null || e.ChangedButton == MouseButton.Left)
            {
                // Открываем диалог выбора изображения
                ImportImage(position);
                if (e != null) e.Handled = true;
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
        /// Импорт изображения с диска
        /// </summary>
        private void ImportImage(Point position)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*",
                Title = "Выберите изображение",
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Создаем BitmapImage с правильными настройками
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Загружаем изображение в память
                    bitmap.CreateOptions = BitmapCreateOptions.None;
                    bitmap.DecodePixelWidth = 1200; // Ограничиваем максимальную ширину для производительности
                    bitmap.EndInit();
                    bitmap.Freeze(); // Делаем его потокобезопасным
                    
                    // Создаем элемент изображения
                    ImageItem imageItem = new ImageItem(bitmap)
                    {
                        FilePath = openFileDialog.FileName, // Сохраняем путь к файлу для будущего использования
                        FileName = Path.GetFileName(openFileDialog.FileName)
                    };
                    
                    // Добавляем элемент на доску через систему истории
                    boardService.AddItemWithHistory(imageItem, position);
                    
                    // Выбираем элемент
                    boardService.SelectItem(imageItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 