using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using OpenCvSharp;
using Point = System.Windows.Point;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace ImageToolWPF
{
    public class RotateRectImageShape : ImageShape
    {
        public RotateRectImageShape()
        {
            _grabThreshold = 5.0;
            DrawPen = new Pen();
            CtrlPen = new Pen();
            CtrlLinePen = new Pen();
            DrawBrush = new SolidColorBrush(Color.FromArgb(100,32, 159, 223));
            DrawPen.Brush = new SolidColorBrush(Color.FromRgb(32, 159, 223));
            
            CtrlBrush = new SolidColorBrush(Color.FromArgb(100,9, 45, 64));
            CtrlPen.Brush = new SolidColorBrush(Color.FromArgb(100,9, 45, 64));
            
            CtrlLineBrush = new SolidColorBrush(Color.FromArgb(100,9, 45, 64));
            CtrlLinePen.Brush = new SolidColorBrush(Color.FromArgb(100,9, 45, 64));
            CtrlLinePen.Thickness = _grabThreshold;
        }

        public Pen DrawPen { get; set; }
        public Pen CtrlPen { get; set; }
        public Brush CtrlLineBrush { get; set; }
        public Pen CtrlLinePen { get; set; }
        public Brush CtrlBrush { get; set; }
        private double _grabThreshold;
        public Brush DrawBrush{ get; set; }
        //private 
        public Point Center {
            set
            {
                _rotatedRect.Center = new Point2f(Convert.ToSingle( value.X),Convert.ToSingle( value.Y));
            }
            get
            {
                return new Point(_rotatedRect.Center.X, _rotatedRect.Center.Y);
            }
         }

        public Size Size
        {
            set
            {
                _rotatedRect.Size = new Size2f(Convert.ToSingle( value.Width),Convert.ToSingle( value.Height));
            }
            get
            {
                return new Size(_rotatedRect.Size.Width,_rotatedRect.Size.Height);
            }
        }

        public float Angle {
            set
            {
                _rotatedRect.Angle = value;
            }
            get
            {
                return _rotatedRect.Angle;
            }
        }
        private RotatedRect _rotatedRect;
        private System.Windows.Point _mouseStart;
        internal override bool UnderMouse(Point mouse, Rect imgRect, Size itemSize)
        {
            base.UnderMouse(mouse, imgRect, itemSize);
            if (imgRect.Width < 1.0 || imgRect.Height < 1.0)
            {
                return false;
            }

            var grabType = GetGrabType(mouse, imgRect, itemSize);
            grabedType = grabType;
            _mouseStart = mouse;
            if (grabType != GrabedType.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private GrabedType grabedType = GrabedType.None;

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            base.Paint(painter, imgRect, itemSize);

            var srcPoly = _rotatedRect.Points();
            var polySharpPnts = new List<System.Windows.Point>();
            foreach (var polyPnt in srcPoly)
            {
                polySharpPnts.Add(MapToPaint(new Point(polyPnt.X, polyPnt.Y), imgRect, itemSize));
            }

            var center = ImageShape.MapToPaint(new Point(_rotatedRect.Center.X,
                _rotatedRect.Center.Y), imgRect, itemSize);
            var sz = ImageShape.MapToPaint(new Size(_rotatedRect.Size.Width,
                _rotatedRect.Size.Height), imgRect, itemSize);
            var tltmp = new Point(center.X - sz.Width / 2.0, center.Y - sz.Height / 2.0);

            var drawRect = new System.Windows.Rect(tltmp, sz);
            var trans = new RotateTransform(_rotatedRect.Angle, center.X, center.Y);

            painter.PushTransform(trans);
            painter.DrawRectangle(DrawBrush, DrawPen, drawRect);
            painter.Pop();


            var d = _grabThreshold;
            switch (grabedType)
            {
                case GrabedType.None:

                    break;
                case GrabedType.LeftEdge:
                {
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[0], d * 2, d * 2);
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[1], d * 2, d * 2);
                    painter.DrawLine(CtrlLinePen, polySharpPnts[0], polySharpPnts[1]);
                }
                    break;
                case GrabedType.TopEdge:
                {
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[2], d * 2, d * 2);
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[1], d * 2, d * 2);
                    painter.DrawLine(CtrlLinePen, polySharpPnts[2], polySharpPnts[1]);
                }
                    break;
                case GrabedType.RightEdge:
                {
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[2], d * 2, d * 2);
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[3], d * 2, d * 2);
                    painter.DrawLine(CtrlLinePen, polySharpPnts[2], polySharpPnts[3]);
                }
                    break;
                case GrabedType.BottomEdge:
                {
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[0], d * 2, d * 2);
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[3], d * 2, d * 2);
                    painter.DrawLine(CtrlLinePen, polySharpPnts[0], polySharpPnts[3]);
                }
                    break;
                case GrabedType.ConerBottomLeft:
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[0], d * 2, d * 2);
                    break;
                case GrabedType.ConerTopLeft:
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[1], d * 2, d * 2);
                    break;
                case GrabedType.ConerTopRight:
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[2], d * 2, d * 2);
                    break;
                case GrabedType.ConerBottomRight:
                    painter.DrawEllipse(CtrlBrush, CtrlPen, polySharpPnts[3], d * 2, d * 2);
                    break;
            }

            double power = 1.0;
            power = imgRect.Width / itemSize.Width;
            String drawStr = "Name:" + Name + "(" + _rotatedRect.Center.X.ToString("0.0") + "," +
                             _rotatedRect.Center.Y.ToString("0.0") + "," +
                             _rotatedRect.Size.Width.ToString("0.0") + "," +
                             _rotatedRect.Size.Height.ToString("0.0") + "," +
                             _rotatedRect.Angle.ToString("0.0") + ")";
            var textScale = 50.0 * power;
            FormattedText formattedText = new FormattedText(
                drawStr,
                CultureInfo.GetCultureInfo("zh-cn"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
          Math.Floor(textScale < 1 ? 1 : textScale),
                DrawPen.Brush);

            formattedText.SetFontFamily("Microsoft YaHei UI");
            var textTrans = new RotateTransform(_rotatedRect.Angle, polySharpPnts[1].X, polySharpPnts[1].Y);
            painter.PushTransform(textTrans);
            
            //formattedText.SetForegroundBrush(DrawBrush);
            painter.DrawText(formattedText, polySharpPnts[1]);
            painter.Pop();
        }

        private enum GrabedType
        {
            None,
            Center,
            TopEdge,
            BottomEdge,
            LeftEdge,
            RightEdge,
            ConerBottomLeft,
            ConerTopLeft,
            ConerTopRight,
            ConerBottomRight
        }
        
        private GrabedType GetGrabType(System.Windows.Point mouse, System.Windows.Rect imgRect, System.Windows.Size itemSize)
        {
            var polySrcPnts = _rotatedRect.Points();
            List<System.Windows.Point> polySharpPnt = new List<System.Windows.Point>();
            foreach (var srcPnt in polySrcPnts)
            {
                polySharpPnt.Add(new Point(srcPnt.X,srcPnt.Y));
            }
            for (int i = 0;i < polySharpPnt.Count;i++)
            {
                polySharpPnt[i] = MapToPaint(polySharpPnt[i], imgRect, itemSize);
                polySrcPnts[i].X = Convert.ToSingle(polySharpPnt[i].X);
                polySrcPnts[i].Y = Convert.ToSingle(polySharpPnt[i].Y);
                if (GetDistance(polySharpPnt[i], mouse) < _grabThreshold)
                {
                    switch (i)
                    {
                        case 0:
                            return GrabedType.ConerBottomLeft;
                        case 1:
                            return GrabedType.ConerTopLeft;
                        case 2:
                            return GrabedType.ConerTopRight;
                        case 3:
                            return GrabedType.ConerBottomRight;
                    }

                }
            }
            if (GetDistance(mouse, polySharpPnt[0],polySharpPnt[1]) < _grabThreshold)
            {
                return GrabedType.LeftEdge;
            }
            if (GetDistance(mouse, polySharpPnt[1],polySharpPnt[2]) < _grabThreshold)
            {
                var dist = GetDistance(mouse, polySharpPnt[1], polySharpPnt[2]);
                return GrabedType.TopEdge;
            }
            if (GetDistance(mouse, polySharpPnt[2],polySharpPnt[3]) < _grabThreshold)
            {
                return GrabedType.RightEdge;
            }
            if (GetDistance(mouse, polySharpPnt[3],polySharpPnt[0]) < _grabThreshold)
            {
                return GrabedType.BottomEdge;
            }
            
            if (Cv2.PointPolygonTest(polySrcPnts, new Point2f(Convert.ToSingle( mouse.X),Convert.ToSingle( mouse.Y)), false) >= 0)
            {
                return GrabedType.Center;
            }
            return GrabedType.None;
        }

        private Tuple<float, float, float> CalcRectMoveVec(int pntIdx1,int pntIdx2,Point mouse, Rect imgRect, Size itemSize)
        {
            var sourceStart = MapToSource(_mouseStart, imgRect, itemSize);
            var sourceEnd = MapToSource(mouse, imgRect, itemSize);
            var srcPoly = _rotatedRect.Points();
            var mouseVec = sourceEnd - sourceStart;
            var lineCenter = srcPoly[pntIdx1] + srcPoly[pntIdx2];
            lineCenter.X /= 2;
            lineCenter.Y /= 2;
            var moveVecTmp = lineCenter - _rotatedRect.Center;
            var moveVec = new Vector(moveVecTmp.X,moveVecTmp.Y) ;
            var moveMod = Math.Sqrt(moveVec.X * moveVec.X + moveVec.Y * moveVec.Y);
            double mouseMod = Math.Sqrt(mouseVec.X * mouseVec.X + mouseVec.Y * mouseVec.Y);
            var angle = Math.Abs(GetAnglePI(moveVec, mouseVec));

            var moveDstMod = Math.Cos(angle) * mouseMod;

            var singleVec = moveVec / moveMod;
            var leftMoveVec = singleVec * moveDstMod;
            var rectMoveVec = singleVec * (moveDstMod / 2.0);
            return new Tuple<float, float, float>(Convert.ToSingle( rectMoveVec.X),Convert.ToSingle(  rectMoveVec.Y),Convert.ToSingle( moveDstMod));
        }
        internal override void MouseMove(Point mouse, Rect imgRect, Size itemSize)
        {
            switch (grabedType)
            {
                case GrabedType.Center:
                {
                    var sourceStart = MapToSource(_mouseStart, imgRect, itemSize);
                    var sourceEnd = MapToSource(mouse, imgRect, itemSize);
                    var sourceVec = sourceEnd - sourceStart;
                    _rotatedRect.Center += new Point2f(Convert.ToSingle(sourceVec.X), Convert.ToSingle(sourceVec.Y));
                    _mouseStart = mouse;
                    if (_parentDisplayer != null)
                    {
                        _parentDisplayer.ImageShapeDataChangedCallBack(this);
                    }
                }
                    break;
                case GrabedType.LeftEdge:
                {
                    var movVec = CalcRectMoveVec(0, 1, mouse, imgRect, itemSize);

                    if ((_rotatedRect.Size.Width + movVec.Item3) >= 1.0)
                    {
                        _rotatedRect.Center.X += movVec.Item1;
                        _rotatedRect.Center.Y += movVec.Item2;

                        _rotatedRect.Size.Width = (_rotatedRect.Size.Width + Convert.ToSingle(movVec.Item3));
                    }
                    else
                    {
                        grabedType = GrabedType.None;
                    }

                    _mouseStart = mouse;
                    if (_parentDisplayer != null)
                    {
                        _parentDisplayer.ImageShapeDataChangedCallBack(this);
                    }
                }
                    break;
                case GrabedType.TopEdge:
                {
                    var movVec = CalcRectMoveVec(1, 2, mouse, imgRect, itemSize);
                    if ((_rotatedRect.Size.Height + movVec.Item3) >= 1.0)
                    {
                        _rotatedRect.Center.X += movVec.Item1;
                        _rotatedRect.Center.Y += movVec.Item2;

                        _rotatedRect.Size.Height = (_rotatedRect.Size.Height + Convert.ToSingle(movVec.Item3));
                    }
                    else
                    {
                        grabedType = GrabedType.None;
                    }

                    _mouseStart = mouse;
                    if (_parentDisplayer != null)
                    {
                        _parentDisplayer.ImageShapeDataChangedCallBack(this);
                    }
                }
                    break;
                case GrabedType.RightEdge:
                {
                    var movVec = CalcRectMoveVec(2, 3, mouse, imgRect, itemSize);
                    if ((_rotatedRect.Size.Width + movVec.Item3) >= 1.0)
                    {
                        _rotatedRect.Center.X += movVec.Item1;
                        _rotatedRect.Center.Y += movVec.Item2;

                        _rotatedRect.Size.Width = (_rotatedRect.Size.Width + Convert.ToSingle(movVec.Item3));
                    }
                    else
                    {
                        grabedType = GrabedType.None;
                    }

                    _mouseStart = mouse;
                    if (_parentDisplayer != null)
                    {
                        _parentDisplayer.ImageShapeDataChangedCallBack(this);
                    }
                }

                    break;
                case GrabedType.BottomEdge:
                {
                    var movVec = CalcRectMoveVec(3, 0, mouse, imgRect, itemSize);
                    if ((_rotatedRect.Size.Height + movVec.Item3) >= 1.0)
                    {
                        _rotatedRect.Center.X += movVec.Item1;
                        _rotatedRect.Center.Y += movVec.Item2;

                        _rotatedRect.Size.Height = (_rotatedRect.Size.Height + Convert.ToSingle(movVec.Item3));
                    }
                    else
                    {
                        grabedType = GrabedType.None;
                    }

                    _mouseStart = mouse;
                    if (_parentDisplayer != null)
                    {
                        _parentDisplayer.ImageShapeDataChangedCallBack(this);
                    }
                }
                    break;
                case GrabedType.ConerTopLeft:
                case GrabedType.ConerTopRight:
                case GrabedType.ConerBottomRight:
                case GrabedType.ConerBottomLeft:
                {
                    var centerSharp = new Point(_rotatedRect.Center.X, _rotatedRect.Center.Y);
                    var sourceStart = MapToSource(_mouseStart, imgRect, itemSize);
                    var sourceEnd = MapToSource(mouse, imgRect, itemSize);
                    var startVec = sourceStart - centerSharp;
                    var endVec = sourceEnd - centerSharp;

                    double angle = 180.0 * (Math.Atan2(endVec.Y, endVec.X) - Math.Atan2(startVec.Y, startVec.X)) /
                                   Math.PI;
                    _rotatedRect.Angle += Convert.ToSingle(angle);
                    _mouseStart = mouse;
                    if (_parentDisplayer != null)
                    {
                        _parentDisplayer.ImageShapeDataChangedCallBack(this);
                    }
                }
                    break;
            }
        }

        internal override void HoverMove(Point mouse, Rect imgRect, Size itemSize)
        {
            base.HoverMove(mouse, imgRect, itemSize);
            grabedType = GetGrabType(mouse,imgRect,itemSize);
        }
    }
}