using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using PBoard.Models;
using PBoard.ViewModels;

namespace PBoard.Views
{
    /// <summary>
    /// Логика взаимодействия для TextStyleDialog.xaml
    /// </summary>
    public partial class TextStyleDialog : Window
    {
        private readonly TextStyleViewModel _viewModel;
        
        public TextStyleViewModel TextStyle => _viewModel;

        /// <summary>
        /// Конструктор диалогового окна с параметрами текущего элемента
        /// </summary>
        public TextStyleDialog()
        {
            InitializeComponent();
            
            _viewModel = new TextStyleViewModel();
            DataContext = _viewModel;
        }
        
        public TextStyleDialog(TextStyleViewModel initialStyle)
        {
            InitializeComponent();
            
            // Создаем копию модели, чтобы не изменять оригинал до подтверждения
            _viewModel = new TextStyleViewModel
            {
                BackgroundColor = initialStyle.BackgroundColor,
                TextColor = initialStyle.TextColor,
                BorderColor = initialStyle.BorderColor,
                BorderThickness = initialStyle.BorderThickness,
                CornerRadius = initialStyle.CornerRadius,
                FontSize = initialStyle.FontSize,
                IsBold = initialStyle.IsBold,
                IsItalic = initialStyle.IsItalic
            };
            
            DataContext = _viewModel;
            
            // Настройка элементов управления
            BorderThicknessSlider.Value = _viewModel.BorderThickness;
            CornerRadiusSlider.Value = _viewModel.CornerRadius;
            FontSizeSlider.Value = _viewModel.FontSize;
            BoldCheckbox.IsChecked = _viewModel.IsBold;
            ItalicCheckbox.IsChecked = _viewModel.IsItalic;
        }

        /// <summary>
        /// Обработчик выбора предустановки стиля
        /// </summary>
        private void StylePresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StylePresets.SelectedItem is ListBoxItem selectedItem)
            {
                string presetName = selectedItem.Content.ToString();
                _viewModel.ApplyPreset(presetName);
                
                // Обновляем слайдеры, так как они не привязаны напрямую
                BorderThicknessSlider.Value = _viewModel.BorderThickness;
                CornerRadiusSlider.Value = _viewModel.CornerRadius;
                FontSizeSlider.Value = _viewModel.FontSize;
                BoldCheckbox.IsChecked = _viewModel.IsBold;
                ItalicCheckbox.IsChecked = _viewModel.IsItalic;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки выбора цвета фона
        /// </summary>
        private void BackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPickerDialog.ShowDialog(this, _viewModel.BackgroundColor, out Color selectedColor))
            {
                _viewModel.BackgroundColor = selectedColor;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки выбора цвета текста
        /// </summary>
        private void TextColor_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPickerDialog.ShowDialog(this, _viewModel.TextColor, out Color selectedColor))
            {
                _viewModel.TextColor = selectedColor;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки выбора цвета границы
        /// </summary>
        private void BorderColor_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPickerDialog.ShowDialog(this, _viewModel.BorderColor, out Color selectedColor))
            {
                _viewModel.BorderColor = selectedColor;
            }
        }
        
        /// <summary>
        /// Обработчик изменения толщины границы
        /// </summary>
        private void BorderThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_viewModel != null)
            {
                _viewModel.BorderThickness = BorderThicknessSlider.Value;
            }
        }

        /// <summary>
        /// Обработчик изменения радиуса скругления
        /// </summary>
        private void CornerRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_viewModel != null)
            {
                _viewModel.CornerRadius = CornerRadiusSlider.Value;
            }
        }

        /// <summary>
        /// Обработчик изменения размера шрифта
        /// </summary>
        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_viewModel != null)
            {
                _viewModel.FontSize = FontSizeSlider.Value;
            }
        }

        /// <summary>
        /// Обработчик изменения стиля шрифта
        /// </summary>
        private void FontStyle_Changed(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.IsBold = BoldCheckbox.IsChecked == true;
                _viewModel.IsItalic = ItalicCheckbox.IsChecked == true;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки ОК
        /// </summary>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Обработчик нажатия кнопки Отмена
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Статический метод для вызова диалога и получения стиля текста
        /// </summary>
        public static bool ShowDialog(Window owner, TextStyleViewModel initialStyle, out TextStyleViewModel resultStyle)
        {
            var dialog = new TextStyleDialog(initialStyle)
            {
                Owner = owner
            };
            
            bool? result = dialog.ShowDialog();
            
            if (result == true)
            {
                resultStyle = dialog.TextStyle;
                return true;
            }
            
            resultStyle = initialStyle;
            return false;
        }
    }
} 