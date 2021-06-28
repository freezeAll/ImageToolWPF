using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using OpenCvSharp;
using Point = System.Windows.Point;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace ImageToolWPF
{
    public class PaintTextObject : PaintObject
    {
        public PaintTextObject() : base()
        {
            _text = "";
            _pixelSize = 12;
        }

        private string _text;

        public string Text { get; set; }

        private Point _origin;
        public Point Origin { get; set; }

        private int _pixelSize;

        public int PixelSize { get; set; }

        internal override void DrawMat(ref Mat paintMat)
        {
            var scale = Cv2.GetFontScaleFromHeight(HersheyFonts.HersheyComplex, _pixelSize);
            var solidColorBrush = DrawPen.Brush as SolidColorBrush;
            Cv2.PutText(paintMat,_text,new OpenCvSharp.Point(_origin.X,_origin.Y) ,HersheyFonts.HersheyComplex,scale,
                new Scalar(solidColorBrush.Color.B,solidColorBrush.Color.G,solidColorBrush.Color.R));
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            double power = 1.0;
            power = imgRect.Width / itemSize.Width;
            var ps = _pixelSize * power;
            FormattedText formattedText = new FormattedText(
                _text,
                CultureInfo.GetCultureInfo("zh-cn"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                Math.Floor(ps < 1 ? 1 : ps),
                DrawPen.Brush);
            painter.DrawText(formattedText, ImageShape.MapToPaint( _origin,imgRect,itemSize));
        }
    }
}