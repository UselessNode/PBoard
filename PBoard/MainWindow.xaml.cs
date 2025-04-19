using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using PBoard.Models;
using PBoard.Services;
using PBoard.Tools;

namespace PBoard;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly BoardService boardService;
    private readonly ToolService toolService;
    private readonly ZoomService zoomService;
    private readonly ProjectService projectService;
    private readonly HistoryService historyService;
    
    public MainWindow()
    {
        InitializeComponent();
        
        // Инициализация сервисов
        boardService = new BoardService(WorkArea);
        toolService = new ToolService();
        zoomService = new ZoomService(WorkArea, WorkAreaScroll);
        projectService = new ProjectService(boardService);
        historyService = new HistoryService();
        
        // Устанавливаем сервис истории в BoardService
        boardService.SetHistoryService(historyService);
        
        // Подписываемся на изменения в истории действий
        historyService.HistoryChanged += HistoryService_HistoryChanged;
        
        // Настройка инструмента выделения с прямоугольником выделения
        SelectionTool selectionTool = new SelectionTool(boardService);
        selectionTool.SetSelectionRectangle(SelectionRectangle);
        
        // Регистрация инструментов
        toolService.RegisterTool(selectionTool);
        toolService.RegisterTool(new RichTextTool(boardService));
        toolService.RegisterTool(new ImageTool(boardService));
        
        // Подписка на события элементов панели инструментов
        SelectionTool.Checked += ToolRadioButton_Checked;
        RichTextTool.Checked += ToolRadioButton_Checked;
        ImageTool.Checked += ToolRadioButton_Checked;
        
        UndoButton.Click += UndoButton_Click;
        RedoButton.Click += RedoButton_Click;
        ExportButton.Click += ExportButton_Click;
        ImportButton.Click += ImportButton_Click;
        
        // Обработка колеса мыши для масштабирования
        WorkAreaScroll.PreviewMouseWheel += WorkAreaScroll_PreviewMouseWheel;
        
        // Подписка на изменение выделения
        boardService.SelectionChanged += BoardService_SelectionChanged;
        
        // Установка начального инструмента
        SelectionTool.IsChecked = true;
        ToolRadioButton_Checked(SelectionTool, new RoutedEventArgs());
        
        // Центрируем вид на рабочей области
        CenterWorkArea();
    }
    
    /// <summary>
    /// Центрирует вид на рабочей области
    /// </summary>
    private void CenterWorkArea()
    {
        // Центрирование произойдет благодаря настройкам Layout в XAML
        // HorizontalAlignment="Center" VerticalAlignment="Center" для Grid, содержащего Canvas
    }
    
    #region Обработчики событий интерфейса
    
    private void ToolRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.Content is string toolName)
        {
            // Активируем выбранный инструмент
            // Особая обработка для форматированного текста, так как мы переименовали кнопку
            string actualToolName = toolName;
            if (toolName == "Текст")
            {
                actualToolName = "Форматированный текст";
            }
            
            toolService.ActivateTool(actualToolName);
            
            // Показываем какой инструмент выбран
            UpdateStatusText($"Инструмент: {toolName}");
        }
    }
    
    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
        // Выполняем отмену последнего действия
        historyService.Undo();
        
        // Обновляем статус
        UpdateStatusText($"Отменено: {historyService.RedoDescription}");
    }
    
    private void RedoButton_Click(object sender, RoutedEventArgs e)
    {
        // Выполняем возврат отмененного действия
        historyService.Redo();
        
        // Обновляем статус
        UpdateStatusText($"Возвращено: {historyService.UndoDescription}");
    }
    
    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        // Создаем диалоговое окно сохранения файла
        Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Сохранить проект",
            Filter = "PBoard проект (*.pbrd)|*.pbrd|Все файлы (*.*)|*.*",
            DefaultExt = ".pbrd",
            AddExtension = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            FileName = "Проект1"
        };
        
        // Показываем диалоговое окно
        if (saveDialog.ShowDialog() == true)
        {
            string filePath = saveDialog.FileName;
            
            // Экспортируем проект
            if (projectService.ExportProject(filePath))
            {
                UpdateStatusText($"Проект успешно экспортирован в {filePath}");
            }
        }
    }
    
    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        // Создаем диалоговое окно открытия файла
        Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Открыть проект",
            Filter = "PBoard проект (*.pbrd)|*.pbrd|Все файлы (*.*)|*.*",
            DefaultExt = ".pbrd",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };
        
        // Показываем диалоговое окно
        if (openDialog.ShowDialog() == true)
        {
            string filePath = openDialog.FileName;
            
            // Импортируем проект
            if (projectService.ImportProject(filePath))
            {
                UpdateStatusText($"Проект успешно импортирован из {filePath}");
            }
        }
    }
    
    private void WorkAreaScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Делегируем обработку сервису масштабирования
        zoomService.OnMouseWheel(e);
        
        // Обновляем заголовок с текущим масштабом
        UpdateZoomInfo();
    }
    
    private void BoardService_SelectionChanged(object? sender, EventArgs e)
    {
        // Обновляем информацию о выделенных элементах
        int selectedCount = boardService.GetSelectedItems().Count;
        
        if (selectedCount == 0)
        {
            UpdateStatusText("Ничего не выбрано");
        }
        else if (selectedCount == 1)
        {
            UpdateStatusText("Выбран 1 элемент");
        }
        else
        {
            UpdateStatusText($"Выбрано {selectedCount} элементов");
        }
    }
    
    #endregion
    
    #region Обработчики событий рабочей области
    
    private void WorkArea_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            // Делегируем обработку сервису масштабирования
            zoomService.StartPanning(e);
        }
        else
        {
            // Получаем точные координаты относительно Canvas
            Point position = e.GetPosition(WorkArea);
            
            // Делегируем обработку активному инструменту
            toolService.OnMouseDown(position, e);
        }
    }
    
    private void WorkArea_MouseMove(object sender, MouseEventArgs e)
    {
        // Делегируем обработку сервису масштабирования для панорамирования
        zoomService.DoPanning(e);
        
        // Получаем точные координаты относительно Canvas
        Point position = e.GetPosition(WorkArea);
        
        // Делегируем обработку активному инструменту
        toolService.OnMouseMove(position, e);
        
        // Отображаем координаты в статусной строке
        UpdateCoordinatesStatus(position);
    }
    
    private void WorkArea_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            // Делегируем обработку сервису масштабирования
            zoomService.StopPanning(e);
        }
        else
        {
            // Получаем точные координаты относительно Canvas
            Point position = e.GetPosition(WorkArea);
            
            // Делегируем обработку активному инструменту
            toolService.OnMouseUp(position, e);
        }
    }
    
    #endregion
    
    #region Вспомогательные методы
    
    private void UpdateZoomInfo()
    {
        double zoomPercentage = Math.Round(zoomService.CurrentZoom * 100);
        Title = $"PBoard - Масштаб: {zoomPercentage}%";
    }
    
    private void UpdateStatusText(string text)
    {
        StatusText.Text = text;
    }
    
    private void UpdateCoordinatesStatus(Point position)
    {
        StatusText.Text = $"X: {Math.Round(position.X)}, Y: {Math.Round(position.Y)}";
    }

    #endregion

    private void RichTextTool_Checked(object sender, RoutedEventArgs e)
    {
        // Активируем инструмент форматированного текста
        if (toolService != null)
        {
            toolService.ActivateTool("Форматированный текст");
            UpdateStatusText("Инструмент: Форматированный текст");
        }
    }

    private void RichTextTool_Checked_1(object sender, RoutedEventArgs e)
    {

    }

    private void HistoryService_HistoryChanged(object sender, EventArgs e)
    {
        // Обновляем доступность кнопок отмены/возврата
        UndoButton.IsEnabled = historyService.CanUndo;
        RedoButton.IsEnabled = historyService.CanRedo;
        
        // Обновляем подсказки
        if (historyService.CanUndo)
            UndoButton.ToolTip = $"Отменить: {historyService.UndoDescription}";
        else
            UndoButton.ToolTip = "Отменить действие (история пуста)";
            
        if (historyService.CanRedo)
            RedoButton.ToolTip = $"Вернуть: {historyService.RedoDescription}";
        else
            RedoButton.ToolTip = "Вернуть отмененное действие (нет отмененных действий)";
    }
}