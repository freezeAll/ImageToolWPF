using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using OpenCvSharp;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;
using Point = System.Windows.Point;
namespace ImageToolWPF
{
    [Serializable]
    public class PaintPathObject : PaintObject
    {
        public PaintPathObject() : base()
        {
            _path = new List<Point>();
            _isPoly = false;
        }

        private List<Point> _path;
        public List<Point> Path {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }
        private bool _isPoly;
        public bool IsPolygon { get; set; }

        internal override void DrawMat(ref Mat paintMat)
        {
            if (_path.Count < 2)
                return;
            var DrawPen = GenDrawPen();
            var brushColor = DrawBrush;
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Cv2.Line(paintMat,new OpenCvSharp.Point(_path[i].X,_path[i].Y),new OpenCvSharp.Point(_path[i + 1].X,_path[i + 1].Y),new Scalar(brushColor.B,brushColor.G,brushColor.R),Convert.ToInt32( DrawPen.Thickness));
            }
            if (_path.Count < 3)
                return;
            if (_isPoly)
            {
                Cv2.Line(paintMat,new OpenCvSharp.Point(_path[0].X,_path[0].Y),new OpenCvSharp.Point(_path[_path.Count - 1].X,_path[_path.Count - 1].Y),new Scalar(brushColor.B,brushColor.G,brushColor.R),Convert.ToInt32( DrawPen.Thickness));
            }
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            if (_path.Count < 2)
                return;
            for (int i = 0; i < _path.Count - 1; i++)
            {
                painter.DrawLine(GenDrawPen() ,ImageShape.MapToPaint(_path[i],imgRect,itemSize),ImageShape.MapToPaint(_path[i + 1],imgRect,itemSize));
            }
            if (_path.Count < 3)
                return;
            if (_isPoly)
            {
                painter.DrawLine(GenDrawPen(), ImageShape.MapToPaint(_path[_path.Count - 1],imgRect,itemSize),ImageShape.MapToPaint(_path[0],imgRect,itemSize));
            }
        }
    }
}