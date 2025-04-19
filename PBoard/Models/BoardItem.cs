using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PBoard.Models
{
    /// <summary>
    /// Базовый класс для всех элементов на доске
    /// </summary>
    public abstract class BoardItem
    {
        public UIElement Element { get; protected set; } = null!;
        public Point Position { get; set; }
        
        // Манипуляторы изменения размера
        public Thumb? TopLeftThumb { get; protected set; }
        public Thumb? TopRightThumb { get; protected set; }
        public Thumb? BottomLeftThumb { get; protected set; }
        public Thumb? BottomRightThumb { get; protected set; }
        // Манипулятор перемещения
        public Thumb? BottomCenterThumb { get; protected set; }
        
        // Флаг выделения
        private bool isSelected = false;
        
        public double Left 
        {
            get => Canvas.GetLeft(Element);
            set => Canvas.SetLeft(Element, value);
        }
        
        public double Top 
        {
            get => Canvas.GetTop(Element);
            set => Canvas.SetTop(Element, value);
        }
        
        public virtual double Width
        {
            get => (Element as FrameworkElement)?.Width ?? 0;
            set 
            {
                if (Element is FrameworkElement fe)
                    fe.Width = value;
            }
        }
        
        public virtual double Height
        {
            get => (Element as FrameworkElement)?.Height ?? 0;
            set 
            {
                if (Element is FrameworkElement fe)
                    fe.Height = value;
            }
        }
        
        public virtual void SetPosition(double x, double y)
        {
            Left = x;
            Top = y;
            Position = new Point(x, y);
            UpdateThumbsPosition();
        }
        
        public virtual void SetSize(double width, double height)
        {
            Width = width;
            Height = height;
            UpdateThumbsPosition();
        }
        
        public virtual void Select()
        {
            if (Element is FrameworkElement frameworkElement)
            {
                frameworkElement.Opacity = 0.8;
            }
            
            isSelected = true;
            ShowResizeThumbs();
        }
        
        public virtual void Deselect()
        {
            if (Element is FrameworkElement frameworkElement)
            {
                frameworkElement.Opacity = 1.0;
            }
            
            isSelected = false;
            HideResizeThumbs();
        }
        
        public bool IsSelected()
        {
            return isSelected;
        }
        
        /// <summary>
        /// Создает манипуляторы изменения размера
        /// </summary>
        public virtual void CreateResizeThumbs(Canvas canvas)
        {
            // Создание манипуляторов
            TopLeftThumb = CreateThumb(canvas);
            TopRightThumb = CreateThumb(canvas);
            BottomLeftThumb = CreateThumb(canvas);
            BottomRightThumb = CreateThumb(canvas);
            BottomCenterThumb = CreateMoveThumb(canvas);
            
            // Назначение обработчиков
            if (TopLeftThumb != null)
            {
                TopLeftThumb.DragDelta += (s, e) => OnTopLeftThumbDrag(e.HorizontalChange, e.VerticalChange);
            }
            
            if (TopRightThumb != null)
            {
                TopRightThumb.DragDelta += (s, e) => OnTopRightThumbDrag(e.HorizontalChange, e.VerticalChange);
            }
            
            if (BottomLeftThumb != null)
            {
                BottomLeftThumb.DragDelta += (s, e) => OnBottomLeftThumbDrag(e.HorizontalChange, e.VerticalChange);
            }
            
            if (BottomRightThumb != null)
            {
                BottomRightThumb.DragDelta += (s, e) => OnBottomRightThumbDrag(e.HorizontalChange, e.VerticalChange);
            }
            
            if (BottomCenterThumb != null)
            {
                BottomCenterThumb.DragDelta += (s, e) => OnBottomCenterThumbDrag(e.HorizontalChange, e.VerticalChange);
            }
            
            // Скрываем манипуляторы по умолчанию
            HideResizeThumbs();
        }
        
        /// <summary>
        /// Создает один манипулятор изменения размера
        /// </summary>
        private Thumb CreateThumb(Canvas canvas)
        {
            Thumb thumb = new Thumb
            {
                Width = 12,
                Height = 12,
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.DarkBlue,
                BorderThickness = new Thickness(2),
                Opacity = 0.9,
                Visibility = Visibility.Collapsed
            };
            
            canvas.Children.Add(thumb);
            Canvas.SetZIndex(thumb, 1000);
            
            return thumb;
        }
        
        /// <summary>
        /// Создает манипулятор перемещения
        /// </summary>
        private Thumb CreateMoveThumb(Canvas canvas)
        {
            Thumb thumb = new Thumb
            {
                Width = 20,
                Height = 20,
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.DodgerBlue,
                BorderThickness = new Thickness(1.5),
                Opacity = 0.9,
                Visibility = Visibility.Collapsed
            };
            
            // Создаем иконку для манипулятора перемещения
            try
            {
                PathGeometry? moveIconGeometry = Application.Current.Resources["MoveIcon"] as PathGeometry;
                
                if (moveIconGeometry != null)
                {
                    Path iconPath = new Path
                    {
                        Data = moveIconGeometry,
                        Stretch = Stretch.Uniform,
                        Fill = System.Windows.Media.Brushes.DodgerBlue,
                        Margin = new Thickness(3)
                    };
                    
                    thumb.Template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = new FrameworkElementFactory(typeof(Grid))
                    };
                    
                    thumb.ApplyTemplate();
                    
                    if (thumb.Template.FindName("grid", thumb) is Grid grid)
                    {
                        grid.Children.Add(new Border 
                        { 
                            Background = System.Windows.Media.Brushes.White,
                            BorderBrush = System.Windows.Media.Brushes.DodgerBlue,
                            BorderThickness = new Thickness(1.5),
                            CornerRadius = new CornerRadius(10)
                        });
                        grid.Children.Add(iconPath);
                    }
                }
            }
            catch
            {
                // Если не удалось создать иконку, оставляем стандартный стиль
            }
            
            canvas.Children.Add(thumb);
            Canvas.SetZIndex(thumb, 1000);
            
            return thumb;
        }
        
        /// <summary>
        /// Показывает манипуляторы изменения размера
        /// </summary>
        protected virtual void ShowResizeThumbs()
        {
            if (TopLeftThumb != null) TopLeftThumb.Visibility = Visibility.Visible;
            if (TopRightThumb != null) TopRightThumb.Visibility = Visibility.Visible;
            if (BottomLeftThumb != null) BottomLeftThumb.Visibility = Visibility.Visible;
            if (BottomRightThumb != null) BottomRightThumb.Visibility = Visibility.Visible;
            if (BottomCenterThumb != null) BottomCenterThumb.Visibility = Visibility.Visible;
            
            UpdateThumbsPosition();
        }
        
        /// <summary>
        /// Скрывает манипуляторы изменения размера
        /// </summary>
        protected virtual void HideResizeThumbs()
        {
            if (TopLeftThumb != null) TopLeftThumb.Visibility = Visibility.Collapsed;
            if (TopRightThumb != null) TopRightThumb.Visibility = Visibility.Collapsed;
            if (BottomLeftThumb != null) BottomLeftThumb.Visibility = Visibility.Collapsed;
            if (BottomRightThumb != null) BottomRightThumb.Visibility = Visibility.Collapsed;
            if (BottomCenterThumb != null) BottomCenterThumb.Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Обновляет позиции манипуляторов изменения размера
        /// </summary>
        public virtual void UpdateThumbsPosition()
        {
            if (Element == null) return;
            
            double left = Left;
            double top = Top;
            double right = left + Width;
            double bottom = top + Height;
            double centerX = left + Width / 2;
            
            if (TopLeftThumb != null)
            {
                Canvas.SetLeft(TopLeftThumb, left - 5);
                Canvas.SetTop(TopLeftThumb, top - 5);
                Canvas.SetZIndex(TopLeftThumb, 1000);
            }
            
            if (TopRightThumb != null)
            {
                Canvas.SetLeft(TopRightThumb, right - 5);
                Canvas.SetTop(TopRightThumb, top - 5);
                Canvas.SetZIndex(TopRightThumb, 1000);
            }
            
            if (BottomLeftThumb != null)
            {
                Canvas.SetLeft(BottomLeftThumb, left - 5);
                Canvas.SetTop(BottomLeftThumb, bottom - 5);
                Canvas.SetZIndex(BottomLeftThumb, 1000);
            }
            
            if (BottomRightThumb != null)
            {
                Canvas.SetLeft(BottomRightThumb, right - 5);
                Canvas.SetTop(BottomRightThumb, bottom - 5);
                Canvas.SetZIndex(BottomRightThumb, 1000);
            }
            
            if (BottomCenterThumb != null)
            {
                Canvas.SetLeft(BottomCenterThumb, centerX - 8);
                Canvas.SetTop(BottomCenterThumb, bottom - 8);
                Canvas.SetZIndex(BottomCenterThumb, 1001); // Чуть выше других манипуляторов
            }
        }
        
        /// <summary>
        /// Обрабатывает перетаскивание верхнего левого манипулятора
        /// </summary>
        protected virtual void OnTopLeftThumbDrag(double horizontalChange, double verticalChange)
        {
            double newLeft = Left + horizontalChange;
            double newTop = Top + verticalChange;
            double newWidth = Width - horizontalChange;
            double newHeight = Height - verticalChange;
            
            // Проверка минимальных размеров
            if (newWidth < 20) 
            {
                newWidth = 20;
                newLeft = Left + Width - 20;
            }
            
            if (newHeight < 20)
            {
                newHeight = 20;
                newTop = Top + Height - 20;
            }
            
            Left = newLeft;
            Top = newTop;
            Width = newWidth;
            Height = newHeight;
            
            UpdateThumbsPosition();
        }
        
        /// <summary>
        /// Обрабатывает перетаскивание верхнего правого манипулятора
        /// </summary>
        protected virtual void OnTopRightThumbDrag(double horizontalChange, double verticalChange)
        {
            double newTop = Top + verticalChange;
            double newWidth = Width + horizontalChange;
            double newHeight = Height - verticalChange;
            
            // Проверка минимальных размеров
            if (newWidth < 20) newWidth = 20;
            if (newHeight < 20)
            {
                newHeight = 20;
                newTop = Top + Height - 20;
            }
            
            Top = newTop;
            Width = newWidth;
            Height = newHeight;
            
            UpdateThumbsPosition();
        }
        
        /// <summary>
        /// Обрабатывает перетаскивание нижнего левого манипулятора
        /// </summary>
        protected virtual void OnBottomLeftThumbDrag(double horizontalChange, double verticalChange)
        {
            double newLeft = Left + horizontalChange;
            double newWidth = Width - horizontalChange;
            double newHeight = Height + verticalChange;
            
            // Проверка минимальных размеров
            if (newWidth < 20)
            {
                newWidth = 20;
                newLeft = Left + Width - 20;
            }
            
            if (newHeight < 20) newHeight = 20;
            
            Left = newLeft;
            Width = newWidth;
            Height = newHeight;
            
            UpdateThumbsPosition();
        }
        
        /// <summary>
        /// Обрабатывает перетаскивание нижнего правого манипулятора
        /// </summary>
        protected virtual void OnBottomRightThumbDrag(double horizontalChange, double verticalChange)
        {
            double newWidth = Width + horizontalChange;
            double newHeight = Height + verticalChange;
            
            // Проверка минимальных размеров
            if (newWidth < 20) newWidth = 20;
            if (newHeight < 20) newHeight = 20;
            
            Width = newWidth;
            Height = newHeight;
            
            UpdateThumbsPosition();
        }
        
        /// <summary>
        /// Обрабатывает перетаскивание центрального нижнего манипулятора (перемещение элемента)
        /// </summary>
        protected virtual void OnBottomCenterThumbDrag(double horizontalChange, double verticalChange)
        {
            // Перемещаем весь элемент
            double newLeft = Left + horizontalChange;
            double newTop = Top + verticalChange;
            
            SetPosition(newLeft, newTop);
        }
        
        /// <summary>
        /// Создает копию элемента
        /// </summary>
        public abstract BoardItem Clone();
    }
} 