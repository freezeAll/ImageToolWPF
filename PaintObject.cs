using System;
using System.Windows.Controls;
using System.Windows.Media;
using OpenCvSharp;

namespace ImageToolWPF
{
    [Serializable]
    public class PaintObject
    {
        public PaintObject()
        {
            _drawPenBrush = Colors.Tomato;
            _drawPenThickness = 1;
            _drawBrush = Colors.Tomato;
        }

        private Color _drawBrush;
        private Color _drawPenBrush;
        private int _drawPenThickness;
        internal virtual void DrawMat(ref Mat paintMat)
        {
        }

        internal virtual void Paint(DrawingContext painter, System.Windows.Rect rect, System.Windows.Size itemSize)
        {
        }

        public int DrawPenThickness
        {
            get
            {
                return _drawPenThickness;
            }
            set
            {
                _drawPenThickness = value;
            }
        }

        protected Pen GenDrawPen()
        {
            var outPen = new Pen();
            outPen.Thickness = _drawPenThickness;
            outPen.Brush = new SolidColorBrush(_drawPenBrush);
            return outPen;
        }

        protected Brush GenDrawBrush()
        {
            return new SolidColorBrush(DrawBrush);
        }

        public Color DrawBrush {
            get
            {
                return _drawBrush;
            }
            set
            {
                _drawBrush = value;
            }
        }

        public Color DrawPenColor
        {
            get
            {
                return _drawPenBrush;
            }
            set
            {
                _drawPenBrush = value;
            }
        }
    }
}