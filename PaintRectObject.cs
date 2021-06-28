using System;
using System.Drawing;
using System.Windows.Media;
using OpenCvSharp;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace ImageToolWPF
{
    public class PaintRectObject : PaintObject
    {
        PaintRectObject() : base()
        {
            DrawBrush = null;
        }
        private Rect _rectSource;

        public Rect PaintRect
        {
            get;
            set;
        }
        internal override void DrawMat(ref Mat paintMat)
        {
            if (DrawBrush != null)
            {
                var brushColor = DrawBrush as SolidColorBrush;
                Cv2.Rectangle(paintMat,
                    new OpenCvSharp.Rect(Convert.ToInt32( _rectSource.X),Convert.ToInt32(_rectSource.Y),Convert.ToInt32(_rectSource.Width),Convert.ToInt32(_rectSource.Height)),
                    new Scalar(brushColor.Color.B,brushColor.Color.G,brushColor.Color.R),-1);
            }
            var penColor = DrawPen.Brush as SolidColorBrush;
            Cv2.Rectangle(paintMat,
                new OpenCvSharp.Rect(Convert.ToInt32( _rectSource.X),Convert.ToInt32(_rectSource.Y),Convert.ToInt32(_rectSource.Width),Convert.ToInt32(_rectSource.Height)),
                new Scalar(penColor.Color.B,penColor.Color.G,penColor.Color.R),Convert.ToInt32( DrawPen.Thickness));
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            var tl = ImageShape.MapToPaint(_rectSource.TopLeft,imgRect,itemSize);
            var br = ImageShape.MapToPaint(_rectSource.BottomRight,imgRect,itemSize);
            painter.DrawRectangle(DrawBrush,DrawPen,new Rect(tl,br));
        }
    }
}