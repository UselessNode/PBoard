using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using PBoard.Components;

namespace PBoard.Models
{
    /// <summary>
    /// Расширенный текстовый элемент с поддержкой форматирования
    /// </summary>
    public class RichTextItem : BoardItem
    {
        private TextContainer _textContainer;
        private bool _isEditing;
        
        /// <summary>
        /// Получает или устанавливает текстовое содержимое
        /// </summary>
        public string Text
        {
            get => _textContainer.TextContent;
            set => _textContainer.TextContent = value;
        }
        
        /// <summary>
        /// Получает или устанавливает форматированное содержимое
        /// </summary>
        public string FormattedContent
        {
            get => _textContainer.FormattedContent;
            set => _textContainer.FormattedContent = value;
        }
        
        /// <summary>
        /// Указывает, находится ли элемент в режиме редактирования
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            private set
            {
                _isEditing = value;
                _textContainer.IsEditing = value;
            }
        }
        
        #region Свойства оформления
        
        /// <summary>
        /// Получает или устанавливает цвет фона текстового контейнера
        /// </summary>
        public System.Windows.Media.Brush BackgroundColor
        {
            get => _textContainer.BackgroundColor;
            set => _textContainer.BackgroundColor = value;
        }
        
        /// <summary>
        /// Получает или устанавливает цвет текста
        /// </summary>
        public System.Windows.Media.Brush TextColor
        {
            get => _textContainer.TextColor;
            set => _textContainer.TextColor = value;
        }
        
        /// <summary>
        /// Получает или устанавливает цвет границы
        /// </summary>
        public System.Windows.Media.Brush BorderColor
        {
            get => _textContainer.BorderColor;
            set => _textContainer.BorderColor = value;
        }
        
        /// <summary>
        /// Получает или устанавливает толщину границы
        /// </summary>
        public double BorderThickness
        {
            get => _textContainer.BorderThicknessValue;
            set => _textContainer.BorderThicknessValue = value;
        }
        
        /// <summary>
        /// Получает или устанавливает радиус скругления углов
        /// </summary>
        public double CornerRadius
        {
            get => _textContainer.CornerRadiusValue;
            set => _textContainer.CornerRadiusValue = value;
        }
        
        /// <summary>
        /// Получает или устанавливает размер шрифта
        /// </summary>
        public double FontSize
        {
            get => _textContainer.FontSizeValue;
            set => _textContainer.FontSizeValue = value;
        }
        
        /// <summary>
        /// Получает или устанавливает семейство шрифта
        /// </summary>
        public System.Windows.Media.FontFamily FontFamily
        {
            get => _textContainer.FontFamilyValue;
            set => _textContainer.FontFamilyValue = value;
        }
        
        /// <summary>
        /// Получает или устанавливает стиль шрифта
        /// </summary>
        public System.Windows.FontStyle FontStyle
        {
            get => _textContainer.FontStyleValue;
            set => _textContainer.FontStyleValue = value;
        }
        
        /// <summary>
        /// Получает или устанавливает толщину шрифта
        /// </summary>
        public System.Windows.FontWeight FontWeight
        {
            get => _textContainer.FontWeightValue;
            set => _textContainer.FontWeightValue = value;
        }
        
        #endregion
        
        /// <summary>
        /// Конструктор с текстом по умолчанию
        /// </summary>
        public RichTextItem(string text = "")
        {
            // Создаем текстовый контейнер
            _textContainer = new TextContainer();
            
            // Устанавливаем начальный текст
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    // Создаем новый документ
                    FlowDocument doc = new FlowDocument();
                    doc.Blocks.Add(new Paragraph(new Run(text)));
                    
                    // Получаем доступ к RichTextBox через отражение и устанавливаем документ
                    if (_textContainer.FindName("TextEditor") is RichTextBox textEditor)
                    {
                        textEditor.Document = doc;
                    }
                    
                    // Устанавливаем текстовое содержимое
                    _textContainer.TextContent = text;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Ошибка при установке текста: " + ex.Message);
                }
            }
            
            // Устанавливаем элемент
            Element = _textContainer;
            
            // Установка минимальных размеров
            Width = 200;
            Height = 100;
            
            // Устанавливаем начальные значения оформления
            _textContainer.FontSizeValue = 14;

            _textContainer.MouseDoubleClick += TextContainer_MouseDoubleClick;
        }
        
        /// <summary>
        /// Переводит текстовый элемент в режим редактирования
        /// </summary>
        public void StartEditing()
        {
            IsEditing = true;
            
            try
            {
                // Получаем доступ к RichTextBox через отражение и устанавливаем фокус
                if (_textContainer.FindName("TextEditor") is RichTextBox textEditor)
                {
                    // Проверяем, имеет ли документ какой-либо текст
                    TextRange range = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
                    
                    // Если текст пустой или содержит только начальный текст, очищаем его при получении фокуса
                    if (string.IsNullOrWhiteSpace(range.Text) || range.Text.Trim() == "Текст контейнера")
                    {
                        textEditor.Document.Blocks.Clear();
                        textEditor.Document.Blocks.Add(new Paragraph(new Run("")));
                    }
                    
                    // Устанавливаем фокус и помещаем курсор в конец текста
                    textEditor.Focus();
                    textEditor.CaretPosition = textEditor.Document.ContentEnd;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Ошибка при установке фокуса: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Завершает режим редактирования текста
        /// </summary>
        public void FinishEditing()
        {
            IsEditing = false;
        }
        
        /// <summary>
        /// Создает манипуляторы изменения размера для расширенного текстового элемента
        /// </summary>
        public override void CreateResizeThumbs(Canvas canvas)
        {
            // Вызываем базовую реализацию для создания манипуляторов
            base.CreateResizeThumbs(canvas);
        }
        
        /// <summary>
        /// Обработка выделения элемента
        /// </summary>
        public override void Select()
        {
            base.Select();
            _textContainer.OnSelected();
        }
        
        /// <summary>
        /// Обработка снятия выделения с элемента
        /// </summary>
        public override void Deselect()
        {
            base.Deselect();
            _textContainer.OnDeselected();
        }
        
        /// <summary>
        /// Создает копию элемента текста
        /// </summary>
        public override BoardItem Clone()
        {
            RichTextItem clone = new RichTextItem();
            clone.FormattedContent = this.FormattedContent;
            clone.SetPosition(this.Left + 20, this.Top + 20); // Смещаем копию
            return clone;
        }
        
        /// <summary>
        /// Применяет выбранный стиль оформления
        /// </summary>
        /// <param name="styleName">Название стиля</param>
        public void ApplyStyle(string styleName)
        {
            switch (styleName)
            {
                case "Стандартный":
                    BackgroundColor = System.Windows.Media.Brushes.White;
                    TextColor = System.Windows.Media.Brushes.Black;
                    BorderColor = System.Windows.Media.Brushes.LightGray;
                    BorderThickness = 1;
                    CornerRadius = 2;
                    FontSize = 14;
                    FontStyle = System.Windows.FontStyles.Normal;
                    FontWeight = System.Windows.FontWeights.Normal;
                    break;
                    
                case "Заметка":
                    BackgroundColor = System.Windows.Media.Brushes.LightYellow;
                    TextColor = System.Windows.Media.Brushes.Black;
                    BorderColor = System.Windows.Media.Brushes.Gold;
                    BorderThickness = 1;
                    CornerRadius = 0;
                    FontSize = 12;
                    FontStyle = System.Windows.FontStyles.Italic;
                    FontWeight = System.Windows.FontWeights.Normal;
                    break;
                    
                case "Важное":
                    BackgroundColor = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(255, 230, 230));
                    TextColor = System.Windows.Media.Brushes.DarkRed;
                    BorderColor = System.Windows.Media.Brushes.Red;
                    BorderThickness = 2;
                    CornerRadius = 4;
                    FontSize = 14;
                    FontStyle = System.Windows.FontStyles.Normal;
                    FontWeight = System.Windows.FontWeights.Bold;
                    break;
                    
                case "Информация":
                    BackgroundColor = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(230, 240, 255));
                    TextColor = System.Windows.Media.Brushes.DarkBlue;
                    BorderColor = System.Windows.Media.Brushes.RoyalBlue;
                    BorderThickness = 1;
                    CornerRadius = 8;
                    FontSize = 13;
                    FontStyle = System.Windows.FontStyles.Normal;
                    FontWeight = System.Windows.FontWeights.Normal;
                    break;
                    
                case "Код":
                    BackgroundColor = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(240, 240, 240));
                    TextColor = System.Windows.Media.Brushes.Black;
                    BorderColor = System.Windows.Media.Brushes.DarkGray;
                    BorderThickness = 1;
                    CornerRadius = 0;
                    FontSize = 13;
                    FontStyle = System.Windows.FontStyles.Normal;
                    FontWeight = System.Windows.FontWeights.Normal;
                    break;
                    
                default:
                    // По умолчанию используем стандартный стиль
                    ApplyStyle("Стандартный");
                    break;
            }
        }

        private void TextContainer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Создаем TextStyleViewModel на основе текущего RichTextItem
            var styleViewModel = TextStyleAdapter.CreateFromRichTextItem(this);
            
            // Получаем родительское окно через визуальное дерево
            Window parentWindow = Window.GetWindow(this.Element);
            
            // Открываем диалог с созданной моделью представления
            if (PBoard.Views.TextStyleDialog.ShowDialog(parentWindow, styleViewModel, out var resultStyle))
            {
                // Применяем результат к текущему RichTextItem
                TextStyleAdapter.ApplyStyleToRichTextItem(resultStyle, this);
            }
            
            e.Handled = true;
        }
    }
} 