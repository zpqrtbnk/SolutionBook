using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SolutionBook
{
    /// <summary>
    /// Represents an adorner for dragging and dropping.
    /// </summary>
    public class DragDropAdorner : Adorner
    {
        // https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/adorners-overview

        private static readonly Pen Pen;

        private int _pos;
        private double _width;
        
        static DragDropAdorner()
        {
            // static variable are initialized in textual order, which can lead
            // to great confusion - better be explicit here.
        
            Brush = new SolidColorBrush(UiColors.DragDropColor);
            Transparent = new SolidColorBrush(Colors.Transparent);
            Pen = new Pen(Brush, 1.2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="pos">The position of the adorner.</param>
        /// <param name="width">The width of the adorner.</param>
        /// <remarks>
        /// <para>The position can be +1 (after the element) or -1 (before the element).</para>
        /// </remarks>
        public DragDropAdorner(UIElement adornedElement, int pos, double width)
            : base(adornedElement)
        {
            IsHitTestVisible = false;
            _pos = pos;
            _width = width;
        }

        /// <summary>
        /// Gets the adorner color brush.
        /// </summary>
        public static readonly Brush Brush;

        /// <summary>
        /// Gets the transparent brush.
        /// </summary>
        public static readonly Brush Transparent;
        protected override void OnRender(DrawingContext drawingContext)
        {
            var height = AdornedElement.DesiredSize.Height;
            height = _pos > 0 ? height : 0;

            var start = new Point(16, height);
            var end = new Point(_width + 4, height);
            
            drawingContext.DrawLine(Pen, start, end);
        }

        /// <summary>
        /// Updates the adorner.
        /// </summary>
        /// <param name="pos">The new position.</param>
        /// <param name="width">The new width.</param>
        /// <remarks>
        /// <para>The adorner </para>
        /// </remarks>
        public void Update(int pos, double width)
        {
            _pos = pos;
            _width = width;
        }
    }
}
