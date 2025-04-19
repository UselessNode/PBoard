using System.Windows;
using PBoard.Models;
using PBoard.Services;

namespace PBoard.Commands
{
    /// <summary>
    /// Команда добавления элемента на доску
    /// </summary>
    public class AddItemCommand : ICommand
    {
        private readonly BoardService boardService;
        private readonly BoardItem item;
        private readonly Point position;
        
        public string Description => "Добавление элемента";
        
        public AddItemCommand(BoardService boardService, BoardItem item, Point position)
        {
            this.boardService = boardService;
            this.item = item;
            this.position = position;
        }
        
        public void Execute()
        {
            boardService.AddItem(item, position);
        }
        
        public void Undo()
        {
            boardService.RemoveItem(item);
        }
    }
    
    /// <summary>
    /// Команда удаления элемента с доски
    /// </summary>
    public class RemoveItemCommand : ICommand
    {
        private readonly BoardService boardService;
        private readonly BoardItem item;
        private readonly Point lastPosition;
        
        public string Description => "Удаление элемента";
        
        public RemoveItemCommand(BoardService boardService, BoardItem item)
        {
            this.boardService = boardService;
            this.item = item;
            this.lastPosition = new Point(item.Left, item.Top);
        }
        
        public void Execute()
        {
            boardService.RemoveItem(item);
        }
        
        public void Undo()
        {
            boardService.AddItem(item, lastPosition);
        }
    }
    
    /// <summary>
    /// Команда перемещения элемента
    /// </summary>
    public class MoveItemCommand : ICommand
    {
        private readonly BoardItem item;
        private readonly double originalX;
        private readonly double originalY;
        private readonly double newX;
        private readonly double newY;
        
        public string Description => "Перемещение элемента";
        
        public MoveItemCommand(BoardItem item, double originalX, double originalY, double newX, double newY)
        {
            this.item = item;
            this.originalX = originalX;
            this.originalY = originalY;
            this.newX = newX;
            this.newY = newY;
        }
        
        public void Execute()
        {
            item.SetPosition(newX, newY);
        }
        
        public void Undo()
        {
            item.SetPosition(originalX, originalY);
        }
    }
    
    /// <summary>
    /// Команда изменения размера элемента
    /// </summary>
    public class ResizeItemCommand : ICommand
    {
        private readonly BoardItem item;
        private readonly double originalWidth;
        private readonly double originalHeight;
        private readonly double newWidth;
        private readonly double newHeight;
        
        public string Description => "Изменение размера";
        
        public ResizeItemCommand(BoardItem item, double originalWidth, double originalHeight, double newWidth, double newHeight)
        {
            this.item = item;
            this.originalWidth = originalWidth;
            this.originalHeight = originalHeight;
            this.newWidth = newWidth;
            this.newHeight = newHeight;
        }
        
        public void Execute()
        {
            item.SetSize(newWidth, newHeight);
        }
        
        public void Undo()
        {
            item.SetSize(originalWidth, originalHeight);
        }
    }
    
    /// <summary>
    /// Команда изменения стиля текстового элемента
    /// </summary>
    public class ChangeTextStyleCommand : ICommand
    {
        private readonly RichTextItem textItem;
        private readonly ViewModels.TextStyleViewModel originalStyle;
        private readonly ViewModels.TextStyleViewModel newStyle;
        
        public string Description => "Изменение стиля текста";
        
        public ChangeTextStyleCommand(RichTextItem textItem, ViewModels.TextStyleViewModel originalStyle, ViewModels.TextStyleViewModel newStyle)
        {
            this.textItem = textItem;
            this.originalStyle = originalStyle;
            this.newStyle = newStyle;
        }
        
        public void Execute()
        {
            TextStyleAdapter.ApplyStyleToRichTextItem(newStyle, textItem);
        }
        
        public void Undo()
        {
            TextStyleAdapter.ApplyStyleToRichTextItem(originalStyle, textItem);
        }
    }
} 