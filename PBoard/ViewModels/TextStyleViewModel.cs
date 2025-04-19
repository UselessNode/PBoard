using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PBoard.ViewModels
{
    public class TextStyleViewModel : INotifyPropertyChanged
    {
        private Color _backgroundColor = Colors.White;
        private Color _textColor = Colors.Black;
        private Color _borderColor = Colors.LightGray;
        private double _borderThickness = 1;
        private double _cornerRadius = 2;
        private double _fontSize = 14;
        private bool _isBold = false;
        private bool _isItalic = false;
        //private bool? _isUnderlined = false;
        //private bool? _isMonospace = false;



        private FontFamily _fontFamily = new FontFamily("Arial");

        public event PropertyChangedEventHandler PropertyChanged;

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackgroundBrush));
                }
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ForegroundBrush));
                }
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BorderBrush));
                }
            }
        }

        public double BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (_borderThickness != value)
                {
                    _borderThickness = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CornerRadius
        {
            get => _cornerRadius;
            set
            {
                if (_cornerRadius != value)
                {
                    _cornerRadius = value;
                    OnPropertyChanged();
                }
            }
        }

        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBold
        {
            get => _isBold;
            set
            {
                if (_isBold != value)
                {
                    _isBold = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FontWeight));
                }
            }
        }

        public FontFamily FontFamily
        {
            get => _fontFamily;
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FontStyle));
                }
            }
        }


        public bool IsItalic
        {
            get => _isItalic;
            set
            {
                if (_isItalic != value)
                {
                    _isItalic = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FontStyle));
                }
            }
        }

        // Calculated properties for bindings
        public SolidColorBrush BackgroundBrush => new SolidColorBrush(BackgroundColor);
        public SolidColorBrush ForegroundBrush => new SolidColorBrush(TextColor);
        public SolidColorBrush BorderBrush => new SolidColorBrush(BorderColor);
        public System.Windows.FontWeight FontWeight => IsBold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal;
        public System.Windows.FontStyle FontStyle => IsItalic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal;



        public void ApplyPreset(string presetName)
        {
            switch (presetName)
            {
                case "Стандартный":
                    BackgroundColor = Colors.White;
                    TextColor = Colors.Black;
                    BorderColor = Colors.LightGray;
                    BorderThickness = 1;
                    CornerRadius = 2;
                    FontSize = 14;
                    IsBold = false;
                    IsItalic = false;
                    FontFamily = new FontFamily("Segoe UI");
                    break;
                case "Заметка":
                    BackgroundColor = Color.FromRgb(255, 255, 204); // Светло-желтый
                    TextColor = Colors.Black;
                    BorderColor = Color.FromRgb(255, 204, 0); // Желтый
                    BorderThickness = 1;
                    CornerRadius = 4;
                    FontSize = 14;
                    IsBold = false;
                    IsItalic = false;
                    FontFamily = new FontFamily("Times New Roman");
                    break;
                case "Важное":
                    BackgroundColor = Color.FromRgb(255, 204, 204); // Светло-красный
                    TextColor = Color.FromRgb(153, 0, 0); // Темно-красный
                    BorderColor = Color.FromRgb(204, 0, 0); // Красный
                    BorderThickness = 2;
                    CornerRadius = 4;
                    FontSize = 14;
                    IsBold = true;
                    IsItalic = false;
                    FontFamily = new FontFamily("Segoe UI");
                    break;
                case "Информация":
                    BackgroundColor = Color.FromRgb(204, 236, 255); // Светло-голубой
                    TextColor = Color.FromRgb(0, 51, 102); // Темно-синий
                    BorderColor = Color.FromRgb(0, 102, 204); // Синий
                    BorderThickness = 1;
                    CornerRadius = 4;
                    FontSize = 14;
                    IsBold = false;
                    IsItalic = false;
                    FontFamily = new FontFamily("Segoe UI");
                    break;
                case "Код":
                    BackgroundColor = Color.FromRgb(240, 240, 240); // Светло-серый
                    TextColor = Colors.Black;
                    BorderColor = Colors.Gray;
                    BorderThickness = 1;
                    CornerRadius = 0;
                    FontSize = 12;
                    IsBold = false;
                    IsItalic = false;
                    FontFamily = new FontFamily("Consolas");
                    break;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 