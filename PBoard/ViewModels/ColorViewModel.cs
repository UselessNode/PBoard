using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PBoard.ViewModels
{
    public class ColorViewModel : INotifyPropertyChanged
    {
        private byte _red;
        private byte _green;
        private byte _blue;
        private byte _alpha = 255;
        private Color _selectedColor;
        private SolidColorBrush _selectedColorBrush;
        private SolidColorBrush _contrastTextBrush;
        private string _hexColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public ColorViewModel()
        {
            // Инициализация черным цветом по умолчанию
            UpdateSelectedColor(Colors.Black);
        }

        public ColorViewModel(Color initialColor)
        {
            UpdateSelectedColor(initialColor);
        }

        public byte Red
        {
            get => _red;
            set
            {
                if (_red != value)
                {
                    _red = value;
                    UpdateSelectedColorFromRgb();
                    OnPropertyChanged();
                }
            }
        }

        public byte Green
        {
            get => _green;
            set
            {
                if (_green != value)
                {
                    _green = value;
                    UpdateSelectedColorFromRgb();
                    OnPropertyChanged();
                }
            }
        }

        public byte Blue
        {
            get => _blue;
            set
            {
                if (_blue != value)
                {
                    _blue = value;
                    UpdateSelectedColorFromRgb();
                    OnPropertyChanged();
                }
            }
        }

        public byte Alpha
        {
            get => _alpha;
            set
            {
                if (_alpha != value)
                {
                    _alpha = value;
                    UpdateSelectedColorFromRgb();
                    OnPropertyChanged();
                }
            }
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    UpdateSelectedColor(value);
                    OnPropertyChanged();
                }
            }
        }

        public SolidColorBrush SelectedColorBrush
        {
            get => _selectedColorBrush;
            private set
            {
                _selectedColorBrush = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush ContrastTextBrush
        {
            get => _contrastTextBrush;
            private set
            {
                _contrastTextBrush = value;
                OnPropertyChanged();
            }
        }

        public string HexColor
        {
            get => _hexColor;
            set
            {
                if (_hexColor != value)
                {
                    _hexColor = value;
                    
                    try
                    {
                        // Проверка на корректность и преобразование hex в Color
                        if (_hexColor.StartsWith("#"))
                            _hexColor = _hexColor.Substring(1);
                            
                        if (_hexColor.Length == 6) // без альфа-канала
                        {
                            var color = (Color)ColorConverter.ConvertFromString("#" + _hexColor);
                            UpdateSelectedColor(color);
                        }
                        else if (_hexColor.Length == 8) // с альфа-каналом
                        {
                            var color = (Color)ColorConverter.ConvertFromString("#" + _hexColor);
                            UpdateSelectedColor(color);
                        }
                    }
                    catch (FormatException)
                    {
                        // Игнорировать ошибки формата при вводе
                    }
                    
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateSelectedColorFromRgb()
        {
            var color = Color.FromArgb(_alpha, _red, _green, _blue);
            UpdateSelectedColor(color, false);
        }

        private void UpdateSelectedColor(Color color, bool updateRgb = true)
        {
            _selectedColor = color;
            
            if (updateRgb)
            {
                _red = color.R;
                _green = color.G;
                _blue = color.B;
                _alpha = color.A;
                
                OnPropertyChanged(nameof(Red));
                OnPropertyChanged(nameof(Green));
                OnPropertyChanged(nameof(Blue));
                OnPropertyChanged(nameof(Alpha));
            }
            
            // Обновление кисти цвета
            SelectedColorBrush = new SolidColorBrush(color);
            
            // Обновление HEX-значения
            _hexColor = color.A == 255 
                ? $"{color.R:X2}{color.G:X2}{color.B:X2}" 
                : $"{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            OnPropertyChanged(nameof(HexColor));
            
            // Определение контрастного цвета для текста
            ContrastTextBrush = IsColorDark(color) 
                ? new SolidColorBrush(Colors.White) 
                : new SolidColorBrush(Colors.Black);
        }

        private bool IsColorDark(Color color)
        {
            // Формула для определения яркости цвета
            // Источник: https://www.w3.org/TR/AERT/#color-contrast
            double brightness = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return brightness < 0.5;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 