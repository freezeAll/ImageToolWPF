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
    [Serializable]
    public class PaintTextObject : PaintObject
    {
        public PaintTextObject() : base()
        {
            _text = "";
            _pixelSize = 12;
        }

        private string _text;

        public string Text {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        private Point _origin;
        public Point Origin {
            get
            {
                return _origin;

            }
            set
            {
                _origin = value;
            }
        }

        private int _pixelSize;

        public int PixelSize {
            get
            {
                return _pixelSize;
            }
            set
            {
                _pixelSize = value;
            }
        }

        internal override void DrawMat(ref Mat paintMat)
        {
            var drawPen = GenDrawPen();
            var scale = Cv2.GetFontScaleFromHeight(HersheyFonts.HersheyComplex, _pixelSize);
            var solidColorBrush = drawPen.Brush as SolidColorBrush;
            Cv2.PutText(paintMat, _text, new OpenCvSharp.Point(_origin.X, _origin.Y), HersheyFonts.HersheyComplex,
                scale, new Scalar(solidColorBrush.Color.B, solidColorBrush.Color.G, solidColorBrush.Color.R));
        }

        internal override void Paint(DrawingContext painter, Rect imgRect, Size itemSize)
        {
            var drawPen = GenDrawPen();

            double power = 1.0;
            power = imgRect.Width / itemSize.Width;
            var ps = _pixelSize * power;
            FormattedText formattedText = new FormattedText(_text, CultureInfo.GetCultureInfo("zh-cn"),
                FlowDirection.LeftToRight, new Typeface("Verdana"), Math.Floor(ps < 1 ? 1 : ps), drawPen.Brush);
            painter.DrawText(formattedText, ImageShape.MapToPaint(_origin, imgRect, itemSize));
        }
    }
}