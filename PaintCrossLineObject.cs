using System;
using System.Drawing;
using System.Windows.Media;
using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace ImageToolWPF
{
    public class PaintCrossLineObject : PaintObject
    {
        public PaintCrossLineObject() : base()
        {
            _lineLength = 5;
            _lineWidth = 1;
        }
        private System.Windows.Point _center;

        public System.Windows.Point Center
        {
            get;
            set;
        }

        private double _lineLength;
        private double _lineWidth;

        public double LineLength
        {
            get;
            set;
        }

        public double LineWidth
        {
            get;
            set;
        }
        internal override void DrawMat(ref Mat paintMat)
        {
            OpenCvSharp.Point horLineL;
            horLineL.X = Convert.ToInt32( _center.X - _lineLength / 2.0);
            horLineL.Y = Convert.ToInt32(_center.Y);
            OpenCvSharp.Point horLineR;
            horLineR.X = Convert.ToInt32(_center.X + _lineLength / 2.0);
            horLineR.Y = Convert.ToInt32(_center.Y);
            OpenCvSharp.Point verLineL;
            verLineL.X = Convert.ToInt32(_center.X) ;
            verLineL.Y = Convert.ToInt32(_center.Y - _lineLength / 2.0);
            OpenCvSharp.Point verLineR;
            verLineR.X = Convert.ToInt32(_center.X) ;
            verLineR.Y = Convert.ToInt32(_center.Y + _lineLength / 2.0);
            var color = DrawPen.Brush as SolidColorBrush;
            Cv2.Line(paintMat,horLineL,horLineR,new Scalar(color.Color.B,color.Color.G,color.Color.R),Convert.ToInt32( DrawPen.Thickness));
            Cv2.Line(paintMat,verLineL,verLineR,new Scalar(color.Color.B,color.Color.G,color.Color.R),Convert.ToInt32(DrawPen.Thickness));
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            System.Windows.Point horLineL;
            horLineL.X = _center.X - _lineLength / 2.0;
            horLineL.Y = _center.Y;
            System.Windows.Point horLineR;
            horLineR.X = _center.X + _lineLength / 2.0;
            horLineR.Y = _center.Y;
            System.Windows.Point verLineL;
            verLineL.X = _center.X ;
            verLineL.Y = _center.Y - _lineLength / 2.0;
            System.Windows.Point verLineR;
            verLineR.X = _center.X ;
            verLineR.Y = _center.Y + _lineLength / 2.0;

            DrawPen.Thickness = _lineWidth;
            
            painter.DrawLine(DrawPen,ImageShape.MapToPaint( horLineL,imgRect,itemSize),ImageShape.MapToPaint( horLineR,imgRect,itemSize));
            painter.DrawLine(DrawPen,ImageShape.MapToPaint( verLineL,imgRect,itemSize),ImageShape.MapToPaint( verLineR,imgRect,itemSize));
        }
    }
}