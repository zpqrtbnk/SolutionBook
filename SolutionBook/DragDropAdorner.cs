using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SolutionBook
{
    public class DragDropAdorner : Adorner
    {
        // https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/adorners-overview

        private static readonly Color Color = Colors.DarkSlateGray;
        public static readonly Brush Brush = new SolidColorBrush(Color);
        public static readonly Brush Transparent = new SolidColorBrush(Colors.Transparent);
        private static readonly Pen Pen = new Pen(Brush, 1.2);

        private int _pos;
        private double _width;

        public DragDropAdorner(UIElement adornedElement, int pos, double width)
            : base(adornedElement)
        {
            IsHitTestVisible = false;
            _pos = pos;
            _width = width;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //var width = AdornedElement.DesiredSize.Width;
            var height = AdornedElement.DesiredSize.Height;
            height = _pos > 0 ? height : 0;
            var start = new Point(16, height);
            var end = new Point(_width + 4, height);
            //var pen = _pos > 0 ? Pen : new Pen(new SolidColorBrush(Colors.DarkRed),1.2);
            drawingContext.DrawLine(Pen, start, end);
        }

        public void UpdatePosition(int pos, double width)
        {
            _pos = pos;
            _width = width;
        }
    }
}
