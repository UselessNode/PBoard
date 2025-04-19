using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PBoard.Models
{
    /// <summary>
    /// Элемент текста на доске
    /// </summary>
    public class TextItem : BoardItem
    {
        private TextBox? _textBox;
        private bool _isEditing;
        
        public string Text
        {
            get => (_textBox != null) ? _textBox.Text : string.Empty;
            set
            {
                if (_textBox != null)
                    _textBox.Text = value;
            }
        }
        
        public bool IsEditing
        {
            get => _isEditing;
            private set
            {
                _isEditing = value;
                if (_textBox != null)
                {
                    _textBox.IsReadOnly = !value;
                    _textBox.Background = value ? Brushes.White : Brushes.Transparent;
                    _textBox.BorderThickness = new Thickness(1);
                    _textBox.BorderBrush = value ? Brushes.Black : Brushes.LightGray;
                }
            }
        }
        
        /// <summary>
        /// Предоставляет доступ к текстовому полю
        /// </summary>
        public TextBox? TextBox => _textBox;
        
        public TextItem(string text = "")
        {
            _textBox = CreateTextBox(text);
            Element = _textBox;
            
            // Назначаем обработчики событий
            _textBox.GotFocus += TextBox_GotFocus;
            _textBox.LostFocus += TextBox_LostFocus;
            _textBox.KeyDown += TextBox_KeyDown;
        }
        
        private TextBox CreateTextBox(string text)
        {
            return new TextBox
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.LightGray,
                MinWidth = 100,
                MinHeight = 30,
                IsReadOnly = true,
                Padding = new Thickness(3)
            };
        }
        
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = true;
        }
        
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
        }
        
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_textBox != null)
                {
                    _textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
        
        /// <summary>
        /// Начинает режим редактирования текста
        /// </summary>
        public void StartEditing()
        {
            IsEditing = true;
            if (_textBox != null)
            {
                _textBox.Focus();
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
        /// Создает манипуляторы изменения размера для текстового элемента
        /// </summary>
        public override void CreateResizeThumbs(Canvas canvas)
        {
            // Вызываем базовую реализацию для создания манипуляторов
            base.CreateResizeThumbs(canvas);
            
            // Дополнительная логика для текстовых элементов при необходимости
        }
        
        /// <summary>
        /// Создает копию элемента текста
        /// </summary>
        public override BoardItem Clone()
        {
            TextItem clone = new TextItem(this.Text);
            clone.SetPosition(this.Left + 20, this.Top + 20); // Смещаем копию
            return clone;
        }
    }
} 