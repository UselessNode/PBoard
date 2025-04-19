using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media;
using PBoard.Models;

namespace PBoard.Components
{
    /// <summary>
    /// Интерфейс для элементов, которые можно добавить в документ как контейнеры
    /// </summary>
    public interface IContainerElement
    {
        bool IsSelected { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Container GetContainer();
    }

    /// <summary>
    /// Контейнер для текстового содержимого
    /// </summary>
    public partial class TextContainer : UserControl, IContainerElement
    {
        // Внутренняя ссылка на контейнер
        private Container _container;

        // Сохраняем оригинальный цвет рамки при выделении
        private Brush _originalBorderBrush;

        #region Свойства

        // Свойство для хранения текста контейнера
        public static readonly DependencyProperty TextContentProperty =
            DependencyProperty.Register("TextContent", typeof(string), typeof(TextContainer),
                new PropertyMetadata("Текст контейнера"));

        // Публичный доступ к тексту контейнера
        public string TextContent
        {
            get { return (string)GetValue(TextContentProperty); }
            set { SetValue(TextContentProperty, value); }
        }

        // Форматированное содержимое в формате XAML
        public string FormattedContent
        {
            get { return GetFormattedContent(); }
            set { SetFormattedContent(value); }
        }

        // Свойство для доступа к полю IsSelected
        public bool IsSelected
        {
            get { return _container.IsSelected; }
            set { _container.IsSelected = value; }
        }

        // Свойство для доступа к полю IsEditing
        public bool IsEditing
        {
            get { return _container.IsEditing; }
            set { _container.IsEditing = value; }
        }

        // Свойство для доступа к Title
        public string Title
        {
            get { return _container.Title; }
            set { _container.Title = value; }
        }

        // Свойство для доступа к X
        public double X
        {
            get { return _container.X; }
            set { _container.X = value; }
        }

        // Свойство для доступа к Y
        public double Y
        {
            get { return _container.Y; }
            set { _container.Y = value; }
        }

        // Получить внутренний контейнер
        public Container GetContainer()
        {
            return _container;
        }
        
        #region Свойства оформления
        
        // Определение свойства зависимости для цвета фона
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(TextContainer),
                new PropertyMetadata(Brushes.White, OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для цвета текста
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Brush), typeof(TextContainer),
                new PropertyMetadata(Brushes.Black, OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для цвета границы
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Brush), typeof(TextContainer),
                new PropertyMetadata(Brushes.LightGray, OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для толщины границы
        public static readonly DependencyProperty BorderThicknessValueProperty =
            DependencyProperty.Register("BorderThicknessValue", typeof(double), typeof(TextContainer),
                new PropertyMetadata(1.0, OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для радиуса скругления углов
        public static readonly DependencyProperty CornerRadiusValueProperty =
            DependencyProperty.Register("CornerRadiusValue", typeof(double), typeof(TextContainer),
                new PropertyMetadata(2.0, OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для семейства шрифта
        public static readonly DependencyProperty FontFamilyValueProperty =
            DependencyProperty.Register("FontFamilyValue", typeof(FontFamily), typeof(TextContainer),
                new PropertyMetadata(new FontFamily("Segoe UI"), OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для размера шрифта
        public static readonly DependencyProperty FontSizeValueProperty =
            DependencyProperty.Register("FontSizeValue", typeof(double), typeof(TextContainer),
                new PropertyMetadata(12.0, OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для стиля шрифта
        public static readonly DependencyProperty FontStyleValueProperty =
            DependencyProperty.Register("FontStyleValue", typeof(FontStyle), typeof(TextContainer),
                new PropertyMetadata(FontStyles.Normal, OnAppearancePropertyChanged));
                
        // Определение свойства зависимости для толщины шрифта
        public static readonly DependencyProperty FontWeightValueProperty =
            DependencyProperty.Register("FontWeightValue", typeof(FontWeight), typeof(TextContainer),
                new PropertyMetadata(FontWeights.Normal, OnAppearancePropertyChanged));
        
        // Цвет фона
        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        
        // Цвет текста
        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
        
        // Цвет границы
        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        
        // Толщина границы
        public double BorderThicknessValue
        {
            get { return (double)GetValue(BorderThicknessValueProperty); }
            set { SetValue(BorderThicknessValueProperty, value); }
        }
        
        // Радиус скругления углов
        public double CornerRadiusValue
        {
            get { return (double)GetValue(CornerRadiusValueProperty); }
            set { SetValue(CornerRadiusValueProperty, value); }
        }
        
        // Семейство шрифта
        public FontFamily FontFamilyValue
        {
            get { return (FontFamily)GetValue(FontFamilyValueProperty); }
            set { SetValue(FontFamilyValueProperty, value); }
        }
        
        // Размер шрифта
        public double FontSizeValue
        {
            get { return (double)GetValue(FontSizeValueProperty); }
            set { SetValue(FontSizeValueProperty, value); }
        }
        
        // Стиль шрифта
        public FontStyle FontStyleValue
        {
            get { return (FontStyle)GetValue(FontStyleValueProperty); }
            set { SetValue(FontStyleValueProperty, value); }
        }
        
        // Толщина шрифта
        public FontWeight FontWeightValue
        {
            get { return (FontWeight)GetValue(FontWeightValueProperty); }
            set { SetValue(FontWeightValueProperty, value); }
        }
        
        // Обработчик изменения свойств оформления
        private static void OnAppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextContainer container)
            {
                container.ApplyAppearance();
            }
        }
        
        // Применение настроек оформления
        public void ApplyAppearance()
        {
            if (ContentBorder != null)
            {
                ContentBorder.Background = BackgroundColor;
                
                // Сохраняем цвет рамки для последующего восстановления после снятия выделения
                _originalBorderBrush = BorderColor;
                
                // Устанавливаем цвет рамки, если элемент не выделен
                if (!IsSelected)
                {
                    ContentBorder.BorderBrush = BorderColor;
                }
                
                ContentBorder.BorderThickness = new Thickness(BorderThicknessValue);
                ContentBorder.CornerRadius = new CornerRadius(CornerRadiusValue);
                ContentBorder.ClipToBounds = true; // Это решит проблему скругления
            }
            
            if (TextEditor != null)
            {
                // Применяем стиль к тексту
                TextEditor.Foreground = TextColor;
                TextEditor.FontSize = FontSizeValue;
                TextEditor.FontStyle = FontStyleValue;
                TextEditor.FontWeight = FontWeightValue;
                TextEditor.FontFamily = FontFamilyValue;
                
                // Применяем стиль ко всему тексту в редакторе
                TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, TextColor);
                range.ApplyPropertyValue(TextElement.FontSizeProperty, FontSizeValue);
                range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyleValue);
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeightValue);
                range.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamilyValue);
            }
        }
        
        #endregion

        #endregion

        #region Конструктор

        public TextContainer()
        {
            InitializeComponent();

            // Создаем и настраиваем контейнер
            _container = new Container();
            
            // Устанавливаем минимальные размеры текстового контейнера
            MinWidth = 100;
            MinHeight = 50;

            // Устанавливаем заголовок контейнера
            _container.Title = "Текстовый блок";

            // Добавляем обработчики для текстового редактора
            TextEditor.PreviewMouseDown += TextEditor_PreviewMouseDown;
            TextEditor.PreviewMouseMove += TextEditor_PreviewMouseMove;
            TextEditor.PreviewMouseUp += TextEditor_PreviewMouseUp;
            
            // Применяем настройки оформления по умолчанию
            ApplyAppearance();
        }

        #endregion

        #region Методы для работы с содержимым

        // Получение форматированного содержимого в виде XAML
        private string GetFormattedContent()
        {
            TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
            
            using (MemoryStream ms = new MemoryStream())
            {
                range.Save(ms, DataFormats.Xaml);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        // Установка форматированного содержимого из XAML
        private void SetFormattedContent(string xamlContent)
        {
            if (string.IsNullOrEmpty(xamlContent))
                return;

            try
            {
                // Создаем новый документ для восстановления содержимого
                FlowDocument doc = new FlowDocument();
                TextRange range = new TextRange(doc.ContentStart, doc.ContentEnd);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms))
                    {
                        sw.Write(xamlContent);
                        sw.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        range.Load(ms, DataFormats.Xaml);
                    }
                }

                // Применяем документ к текстовому редактору
                TextEditor.Document = doc;
            }
            catch (Exception ex)
            {
                // В случае ошибки загрузки XAML, устанавливаем простой текст
                TextEditor.Document.Blocks.Clear();
                TextEditor.Document.Blocks.Add(new Paragraph(new Run("Ошибка загрузки содержимого: " + ex.Message)));
            }
        }

        // Обновляет текстовое свойство из RichTextBox
        private void UpdateTextContent()
        {
            TextRange range = new TextRange(TextEditor.Document.ContentStart, TextEditor.Document.ContentEnd);
            TextContent = range.Text.TrimEnd('\r', '\n');
            
            // Вызываем событие изменения контейнера
            OnContentChanged();
        }

        #endregion

        #region События TextEditor

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTextContent();
        }

        private void TextEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            // Переводим контейнер в режим редактирования
            IsEditing = true;
        }

        private void TextEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            // Выходим из режима редактирования
            IsEditing = false;
        }

        private void TextEditor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Предотвращаем запуск перетаскивания во время редактирования текста
            e.Handled = false;
        }

        private void TextEditor_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Предотвращаем запуск перетаскивания во время редактирования текста, если не нажата правая кнопка
            if (e.RightButton != MouseButtonState.Pressed)
                e.Handled = false;
        }

        private void TextEditor_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Предотвращаем запуск перетаскивания во время редактирования текста
            e.Handled = false;
        }

        #endregion

        #region Методы базового контейнера

        protected void OnContentChanged()
        {
            // Дополнительные действия при изменении содержимого
        }
        
        // Метод для выделения контейнера
        public void OnSelected()
        {
            // Запоминаем оригинальный цвет рамки
            _originalBorderBrush = ContentBorder.BorderBrush;
            
            // Делаем рамку редактора заметной при выделении
            Brush selectionBrush = new SolidColorBrush(Color.FromArgb(255, 48, 120, 215));
            ContentBorder.BorderBrush = selectionBrush;
        }

        // Метод для снятия выделения
        public void OnDeselected()
        {
            // Возвращаем оригинальный цвет рамки
            ContentBorder.BorderBrush = _originalBorderBrush ?? BorderColor;
        }
        #endregion
    }
} 