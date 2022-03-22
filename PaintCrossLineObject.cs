using System;
using System.Drawing;
using System.Windows.Media;
using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace ImageToolWPF
{
    [Serializable]
    public class PaintCrossLineObject : PaintObject
    {
        public PaintCrossLineObject() : base()
        {
            _lineLength = 5;
            _lineWidth = 1;
        }

        private System.Windows.Point _center;

        public System.Windows.Point Center {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
            }
        }

        private double _lineLength;
        private double _lineWidth;

        public double LineLength
        {
            get
            {
                return _lineLength;

            }
            set
            {
                _lineLength = value;
            }
        }

        public double LineWidth
        {
            get
            {
                return _lineWidth;

            }
            set
            {
                _lineWidth = value;
            }
        }

        internal override void DrawMat(ref Mat paintMat)
        {
            var drawPen = GenDrawPen();

            OpenCvSharp.Point horLineL;
            horLineL.X = Convert.ToInt32(_center.X - _lineLength / 2.0);
            horLineL.Y = Convert.ToInt32(_center.Y);
            OpenCvSharp.Point horLineR;
            horLineR.X = Convert.ToInt32(_center.X + _lineLength / 2.0);
            horLineR.Y = Convert.ToInt32(_center.Y);
            OpenCvSharp.Point verLineL;
            verLineL.X = Convert.ToInt32(_center.X);
            verLineL.Y = Convert.ToInt32(_center.Y - _lineLength / 2.0);
            OpenCvSharp.Point verLineR;
            verLineR.X = Convert.ToInt32(_center.X);
            verLineR.Y = Convert.ToInt32(_center.Y + _lineLength / 2.0);
            var color = drawPen.Brush as SolidColorBrush;
            Cv2.Line(paintMat, horLineL, horLineR, new Scalar(color.Color.B, color.Color.G, color.Color.R),
                Convert.ToInt32(drawPen.Thickness));
            Cv2.Line(paintMat, verLineL, verLineR, new Scalar(color.Color.B, color.Color.G, color.Color.R),
                Convert.ToInt32(drawPen.Thickness));
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            var drawPen = GenDrawPen();

            System.Windows.Point horLineL = new System.Windows.Point();
            horLineL.X = _center.X - _lineLength / 2.0;
            horLineL.Y = _center.Y;
            System.Windows.Point horLineR = new System.Windows.Point();
            horLineR.X = _center.X + _lineLength / 2.0;
            horLineR.Y = _center.Y;
            System.Windows.Point verLineL = new System.Windows.Point();
            verLineL.X = _center.X;
            verLineL.Y = _center.Y - _lineLength / 2.0;
            System.Windows.Point verLineR = new System.Windows.Point();
            verLineR.X = _center.X;
            verLineR.Y = _center.Y + _lineLength / 2.0;

            drawPen.Thickness = _lineWidth;

            painter.DrawLine(drawPen, ImageShape.MapToPaint(horLineL, imgRect, itemSize),
                ImageShape.MapToPaint(horLineR, imgRect, itemSize));
            painter.DrawLine(drawPen, ImageShape.MapToPaint(verLineL, imgRect, itemSize),
                ImageShape.MapToPaint(verLineR, imgRect, itemSize));
        }
    }
}