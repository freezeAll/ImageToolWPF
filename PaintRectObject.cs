using System;
using System.Drawing;
using System.Windows.Media;
using OpenCvSharp;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace ImageToolWPF
{
    [Serializable]
    public class PaintRectObject : PaintObject
    {
        PaintRectObject() : base()
        {
        }

        private Rect _rectSource;

        public Rect PaintRect {
            get
            {
                return _rectSource;
            }
            set
            {
                _rectSource = value;
            }
        }

        internal override void DrawMat(ref Mat paintMat)
        {
            var drawBrush = GenDrawBrush();
            if (DrawBrush != null)
            {
                var brushColor = drawBrush as SolidColorBrush;
                Cv2.Rectangle(paintMat,
                    new OpenCvSharp.Rect(Convert.ToInt32(_rectSource.X), Convert.ToInt32(_rectSource.Y),
                        Convert.ToInt32(_rectSource.Width), Convert.ToInt32(_rectSource.Height)),
                    new Scalar(brushColor.Color.B, brushColor.Color.G, brushColor.Color.R), -1);
            }

            var drawPen = GenDrawPen();

            var penColor = drawPen.Brush as SolidColorBrush;
            Cv2.Rectangle(paintMat,
                new OpenCvSharp.Rect(Convert.ToInt32(_rectSource.X), Convert.ToInt32(_rectSource.Y),
                    Convert.ToInt32(_rectSource.Width), Convert.ToInt32(_rectSource.Height)),
                new Scalar(penColor.Color.B, penColor.Color.G, penColor.Color.R), Convert.ToInt32(drawPen.Thickness));
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            var drawPen = GenDrawPen();
            var drawBrush = GenDrawBrush();

            var tl = ImageShape.MapToPaint(_rectSource.TopLeft, imgRect, itemSize);
            var br = ImageShape.MapToPaint(_rectSource.BottomRight, imgRect, itemSize);
            painter.DrawRectangle(drawBrush, drawPen, new Rect(tl, br));
        }
    }
}