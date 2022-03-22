using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using OpenCvSharp;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;


namespace ImageToolWPF
{
    [Serializable]
    public class PaintLineObject : PaintObject
    {
        public PaintLineObject() : base()
        {
        }

        private System.Windows.Point _point1;
        private System.Windows.Point _point2;

        public System.Windows.Point StartPoint {
            get
            {
                return _point1;
            }
            set
            {
                _point1 = value;
            }
        }
        public System.Windows.Point EndPoint {
            get
            {
                return _point2;
            }
            set
            {
                _point2 = value;
            }
        }

        internal override void DrawMat(ref Mat paintMat)
        {
            var drawPen = GenDrawPen();
            var penColor = drawPen.Brush as SolidColorBrush;
            Cv2.Line(paintMat,
                new OpenCvSharp.Point(Convert.ToInt32(_point1.X), Convert.ToInt32(_point1.Y)),
                new OpenCvSharp.Point(Convert.ToInt32(_point2.X), Convert.ToInt32(_point2.Y)),
                new Scalar(penColor.Color.B, penColor.Color.G, penColor.Color.R), Convert.ToInt32(drawPen.Thickness));
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            var sp = ImageShape.MapToPaint(StartPoint, imgRect, itemSize);
            var ep = ImageShape.MapToPaint(EndPoint, imgRect, itemSize);
            
            painter.DrawLine(GenDrawPen(), sp, ep);
        }
    }
}
