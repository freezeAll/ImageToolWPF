using System.Windows.Media;
using OpenCvSharp;
namespace ImageToolWPF
{
    public class PaintObject
    {
        public PaintObject()
        {
            _drawPen = new Pen();
            _drawPen.Thickness = 1;
            _drawPen.Brush = new SolidColorBrush(Colors.Tomato);
            _drawBrush = new SolidColorBrush(Colors.Tomato);
        }
        private Pen _drawPen;
        private Brush _drawBrush;

        internal virtual void DrawMat(ref Mat paintMat)
        {
            
        }
        internal virtual void Paint(DrawingContext painter,System.Windows.Rect rect,System.Windows.Size itemSize)
        {
            
        }
        public Pen DrawPen
        {
            get;
            set;
        }
        public Brush DrawBrush
        {
            get;
            set;
        }

        public Color DrawPenColor
        {
            get
            {
                var penBrush = DrawPen.Brush as SolidColorBrush;
                return penBrush.Color;
            }
            set
            {
                DrawPen.Brush = new SolidColorBrush(value);
            }
        }
        
        public Color DrawBrushColor
        {
            get
            {
                var brush = DrawBrush as SolidColorBrush;
                return brush.Color;
            }
            set
            {
                _drawBrush = new SolidColorBrush(value);
            }
        }
        
    }
}