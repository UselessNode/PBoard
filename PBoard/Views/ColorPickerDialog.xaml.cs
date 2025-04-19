using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PBoard.ViewModels;

namespace PBoard.Views
{
    public partial class ColorPickerDialog : Window
    {
        private readonly ColorViewModel _viewModel;
        private bool _updatingHexFromRgb = false;
        private static readonly ObservableCollection<SolidColorBrush> _recentColors = new ObservableCollection<SolidColorBrush>();
        
        public Color SelectedColor => _viewModel.SelectedColor;

        public ColorPickerDialog(Color initialColor)
        {
            InitializeComponent();
            
            _viewModel = new ColorViewModel(initialColor);
            DataContext = _viewModel;
            
            // Инициализация стандартных цветов
            LoadStandardColors();
            
            // Инициализация недавно использованных цветов
            RecentColors.ItemsSource = _recentColors;
        }
        
        private void LoadStandardColors()
        {
            var standardColors = new List<SolidColorBrush>
            {
                // Первый ряд - оттенки серого
                new SolidColorBrush(Colors.Black),
                new SolidColorBrush(Colors.DimGray),
                new SolidColorBrush(Colors.Gray),
                new SolidColorBrush(Colors.DarkGray),
                new SolidColorBrush(Colors.Silver),
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.Gainsboro),
                new SolidColorBrush(Colors.White),
                
                // Красные
                new SolidColorBrush(Colors.DarkRed),
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Firebrick),
                new SolidColorBrush(Colors.Crimson),
                new SolidColorBrush(Colors.IndianRed),
                new SolidColorBrush(Colors.LightCoral),
                new SolidColorBrush(Colors.Salmon),
                new SolidColorBrush(Colors.LightSalmon),
                
                // Оранжевые
                new SolidColorBrush(Colors.DarkOrange),
                new SolidColorBrush(Colors.Orange),
                new SolidColorBrush(Colors.OrangeRed),
                new SolidColorBrush(Colors.Tomato),
                new SolidColorBrush(Colors.Coral),
                new SolidColorBrush(Colors.SandyBrown),
                new SolidColorBrush(Colors.Peru),
                new SolidColorBrush(Colors.Chocolate),
                
                // Желтые
                new SolidColorBrush(Colors.DarkGoldenrod),
                new SolidColorBrush(Colors.Goldenrod),
                new SolidColorBrush(Colors.Gold),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Khaki),
                new SolidColorBrush(Colors.LemonChiffon),
                new SolidColorBrush(Colors.LightYellow),
                new SolidColorBrush(Colors.PaleGoldenrod),
                
                // Зеленые
                new SolidColorBrush(Colors.DarkGreen),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.ForestGreen),
                new SolidColorBrush(Colors.SeaGreen),
                new SolidColorBrush(Colors.MediumSeaGreen),
                new SolidColorBrush(Colors.LimeGreen),
                new SolidColorBrush(Colors.Lime),
                new SolidColorBrush(Colors.LightGreen),
                
                // Синие
                new SolidColorBrush(Colors.Navy),
                new SolidColorBrush(Colors.DarkBlue),
                new SolidColorBrush(Colors.MediumBlue),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.RoyalBlue),
                new SolidColorBrush(Colors.SteelBlue),
                new SolidColorBrush(Colors.DeepSkyBlue),
                new SolidColorBrush(Colors.SkyBlue)
            };
            
            StandardColors.ItemsSource = standardColors;
        }
        
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var brush = button?.Background as SolidColorBrush;
            
            if (brush != null)
            {
                _viewModel.SelectedColor = brush.Color;
                AddToRecentColors(brush.Color);
            }
        }
        
        private void RgbSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // RGB-слайдеры уже связаны с ViewModel через привязку данных
            // Здесь можно добавить дополнительную логику, если нужно
        }
        
        private void HexColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Избегаем циклической обратной связи во время обновления
            if (_updatingHexFromRgb) 
                return;
                
            // Привязка данных уже обновит ViewModel
            // Ничего делать не нужно, т.к. уже есть привязка с UpdateSourceTrigger=PropertyChanged
        }
        
        private void AddToRecentColors(Color color)
        {
            // Добавление цвета в список недавно использованных
            var brush = new SolidColorBrush(color);
            
            // Проверяем, есть ли уже такой цвет в списке
            for (int i = 0; i < _recentColors.Count; i++)
            {
                if (_recentColors[i].Color.Equals(color))
                {
                    _recentColors.RemoveAt(i);
                    break;
                }
            }
            
            // Добавляем в начало списка
            _recentColors.Insert(0, brush);
            
            // Ограничиваем количество недавних цветов
            while (_recentColors.Count > 10)
            {
                _recentColors.RemoveAt(_recentColors.Count - 1);
            }
        }
        
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        /// <summary>
        /// Статический метод для вызова диалога и получения выбранного цвета
        /// </summary>
        public static bool ShowDialog(Window owner, Color initialColor, out Color selectedColor)
        {
            var dialog = new ColorPickerDialog(initialColor)
            {
                Owner = owner
            };
            
            bool? result = dialog.ShowDialog();
            
            if (result == true)
            {
                selectedColor = dialog.SelectedColor;
                return true;
            }
            
            selectedColor = initialColor;
            return false;
        }
    }
} 