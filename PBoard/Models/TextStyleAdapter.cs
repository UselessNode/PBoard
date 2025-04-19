using System.Windows.Media;
using PBoard.Components;
using PBoard.ViewModels;

namespace PBoard.Models
{
    /// <summary>
    /// Класс-адаптер для преобразования между RichTextItem и TextStyleViewModel
    /// </summary>
    public static class TextStyleAdapter
    {
        /// <summary>
        /// Создает TextStyleViewModel на основе RichTextItem
        /// </summary>
        public static TextStyleViewModel CreateFromRichTextItem(RichTextItem item)
        {
            var viewModel = new TextStyleViewModel();
            
            // Преобразование Brush в Color
            if (item.BackgroundColor is SolidColorBrush bgBrush)
                viewModel.BackgroundColor = bgBrush.Color;
                
            if (item.TextColor is SolidColorBrush textBrush)
                viewModel.TextColor = textBrush.Color;
                
            if (item.BorderColor is SolidColorBrush borderBrush)
                viewModel.BorderColor = borderBrush.Color;
            
            // Копирование остальных свойств
            viewModel.BorderThickness = item.BorderThickness;
            viewModel.CornerRadius = item.CornerRadius;
            viewModel.FontSize = item.FontSize;
            viewModel.IsItalic = item.FontStyle == System.Windows.FontStyles.Italic;
            viewModel.IsBold = item.FontWeight == System.Windows.FontWeights.Bold;
            viewModel.FontFamily = item.FontFamily;
            
            return viewModel;
        }
        
        /// <summary>
        /// Применяет стиль из TextStyleViewModel к RichTextItem
        /// </summary>
        public static void ApplyStyleToRichTextItem(TextStyleViewModel style, RichTextItem item)
        {
            // Создаем новые SolidColorBrush для каждого свойства
            item.BackgroundColor = new SolidColorBrush(style.BackgroundColor);
            item.TextColor = new SolidColorBrush(style.TextColor);
            item.BorderColor = new SolidColorBrush(style.BorderColor);
            
            // Применяем числовые параметры
            item.BorderThickness = style.BorderThickness;
            item.CornerRadius = style.CornerRadius;
            
            // Устанавливаем размер шрифта (важно для исправления проблемы с размером)
            item.FontSize = style.FontSize;
            
            // Применяем семейство шрифта
            item.FontFamily = style.FontFamily;
            
            // Применяем стиль шрифта
            item.FontStyle = style.IsItalic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal;
            item.FontWeight = style.IsBold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal;
            
            // Принудительное обновление визуального представления
            if (item.Element is TextContainer container)
            {
                container.ApplyAppearance();
            }
        }
    }
} 