using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenCvSharp;
using Image = System.Drawing.Image;
using Rect = System.Windows.Rect;

namespace ImageToolWPF
{
    public partial class ImageToolDisplayer : UserControl
    {
        public ImageToolDisplayer()
        {
            InitializeComponent();
            _paintData = new ImageToolPaintData();
            _sourceImage = null;
            Shapes = new List<ImageShape>();
            
            Clip = new RectangleGeometry(new Rect(0, 0, masterCanva.Width, masterCanva.Height));
            _displayArea = new Rect();

            var wbinding = new Binding();
            wbinding.Source = this;
            wbinding.Path = new PropertyPath("Width");
            masterCanva.SetBinding(Canvas.WidthProperty, wbinding);
            var hbinding = new Binding();
            hbinding.Source = this;
            hbinding.Path = new PropertyPath("Height");
            masterCanva.SetBinding(Canvas.HeightProperty, hbinding);
        }

        public void PushShape(ImageShape shape)
        {
            shape._parentDisplayer = this;
            Shapes.Add(shape);
            InvalidateVisual();
        }
        private BitmapImage _sourceImage;

        public static readonly DependencyProperty CvMatShowImageProperty = DependencyProperty.Register("CvMatShowImage", typeof(Mat), typeof(ImageToolDisplayer),new PropertyMetadata(null, CvMatShowImagePropertyChangedCallback, null));

        private static void CvMatShowImagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ImageToolDisplayer;
            if(obj != null)
                obj.CvMatShowImage = e.NewValue as Mat ;
        }

        public Mat CvMatShowImage
        {
            get
            {
                var val = GetValue(CvMatShowImageProperty);
                return val as Mat;
            }
            set
            {
                SetValue(CvMatShowImageProperty,value);
                SetShowImage(value);
            }
        }
        private Rect _displayArea;
        private System.Windows.Point _startPos;
        
        public static readonly DependencyProperty ImageShapesProperty = DependencyProperty.Register("ImageShapes", typeof(List<ImageShape>), typeof(ImageToolDisplayer),new PropertyMetadata(null, ImageShapesPropertyChangedCallback, null));

        private static void ImageShapesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ImageToolDisplayer;
            if (obj != null)
            {
                obj.Shapes.Clear();
                foreach (var newVal in e.NewValue as List<ImageShape>)
                {
                    newVal._parentDisplayer = obj;
                    obj.Shapes.Add(newVal);
                }
                obj.InvalidateVisual();
            }
        }
    
        

        private List<ImageShape> Shapes;

        public List<ImageShape> ImageShapes
        {
            get
            {
                return GetValue(ImageShapesProperty) as List<ImageShape>;
            }
            set
            {
                foreach(var shape in value)
                {
                    shape._parentDisplayer = this;
                }
                Shapes = value;
                SetValue(ImageShapesProperty,value);
                InvalidateVisual();
            }
        }

        public static readonly DependencyProperty PaintObjectsProperty = DependencyProperty.Register("PaintObject", typeof(List<PaintObject>), typeof(ImageToolDisplayer),new PropertyMetadata(null, PaintObjectsPropertyChangedCallback, null));

        private static void PaintObjectsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ImageToolDisplayer;
            if (obj != null)
            {
                obj.PaintObjects.Clear();
                foreach (var newVal in e.NewValue as List<PaintObject>)
                {
                    obj.PaintObjects.Add(newVal);
                }
                obj.InvalidateVisual();
            }
        }

        public List<PaintObject> PaintObjects
        {
            get
            {
                return _paintData.PaintObjects;
            }
            set
            {
                SetValue(PaintObjectsProperty,value);
                _paintData.PaintObjects = value;
                InvalidateVisual();
            }
        }
        private void SetShowImage(Mat matImg)
        {
            var bm = new BitmapImage();
            bm.BeginInit();
            bm.StreamSource = new MemoryStream(matImg.ToBytes());
            bm.EndInit();
            _sourceImage = bm;
            ResetScale();
            InvalidateVisual();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            ResetScale();
            InvalidateVisual();
        }
        

        private ImageShape grabedShape;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (_sourceImage == null)
            {
                return;
            }
            var pos = e.GetPosition(masterCanva);
            _startPos = pos;
            var itemSize = new System.Windows.Size(_sourceImage.Width, _sourceImage.Height);
            foreach (var shape in Shapes)
            {
                if (shape.UnderMouse(pos, _displayArea, itemSize))
                {
                    grabedShape = shape;
                    break;
                }
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            grabedShape = null;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_sourceImage == null)
            {
                return;
            }
            var pos = e.GetPosition(masterCanva);
            var itemSize = new System.Windows.Size(_sourceImage.Width, _sourceImage.Height);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (grabedShape != null)
                {
                    grabedShape.MouseMove(pos,_displayArea,itemSize);
                    InvalidateVisual();
                    return;
                }
                var vec = pos - _startPos;
                _displayArea.X += vec.X;
                _displayArea.Y += vec.Y;
                _startPos = pos;
                InvalidateVisual();
            }
            else
            {
                foreach (var shape in Shapes)
                {
                    shape.HoverMove(pos,_displayArea,new System.Windows.Size(_sourceImage.Width,_sourceImage.Height));
                }
                InvalidateVisual();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var itemWidth = masterCanva.Width;
            var itemHeight = masterCanva.Height;
            
            double numDegrees = e.Delta;
            var pos = e.GetPosition(masterCanva);
            System.Windows.Point transP;
            System.Windows.Size areaSize = _displayArea.Size;
            ScaleTransform trans;
            if (numDegrees < 0)
            {
                trans = new ScaleTransform(0.95, 0.95, pos.X, pos.Y);
                var tl = _displayArea.TopLeft;
                transP = trans.Transform(tl);
                areaSize.Width *= 0.95;
                areaSize.Height *= 0.95;
            }
            else
            {
                trans = new ScaleTransform(1.05, 1.05, pos.X, pos.Y);
                var tl = _displayArea.TopLeft;
                transP = trans.Transform(tl);
                areaSize.Width *= 1.05;
                areaSize.Height *= 1.05;
            }
            _displayArea = new Rect(transP.X, transP.Y, areaSize.Width, areaSize.Height);
            InvalidateVisual();
        }

        private void ResetScale()
        {
            if (_sourceImage == null)
                return;
            var itemWidth = masterCanva.Width;
            var itemHeight = masterCanva.Height;
            if (itemWidth >= 1.0 && itemHeight >= 1.0)
            {
                if (_sourceImage.Width / _sourceImage.Height > itemWidth / itemHeight)
                {
                    double power = _sourceImage.Width / itemWidth;
                    _displayArea.X = 0;
                    _displayArea.Width = itemWidth;
                    _displayArea.Height = _sourceImage.Height / power;
                    _displayArea.Y = (itemHeight - _displayArea.Height) / 2.0;
                }
                else
                {
                    double power = _sourceImage.Height / itemHeight;
                    _displayArea.Y = 0;
                    _displayArea.Width = _sourceImage.Width / power;
                    _displayArea.Height = itemHeight;
                    _displayArea.X = (itemWidth - _displayArea.Width) / 2.0;
                }
            }
        }

        public void PushPaintObject(PaintObject obj)
        {
            _paintData.PaintObjects.Add(obj);
            InvalidateVisual();
        }
        public void ClearPaintObject()
        {
            _paintData.PaintObjects.Clear();
            InvalidateVisual();
        }
        private ImageToolPaintData _paintData;

        public void DrawMat(ref Mat paintMat)
        {
            _paintData.DrawMat(ref paintMat);
        }
        public ImageToolPaintData PaintData
        {
            get
            {
                return _paintData;
            }
            set
            {
                _paintData = value;
                InvalidateVisual();
            }
        }

        public void ResetImageScale()
        {
            ResetScale();
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            
            SolidColorBrush mySolidColorBrush  = new SolidColorBrush();
            mySolidColorBrush.Color = Colors.LightGray;
            Pen myPen = new Pen(Brushes.LightGray, 0);
            Rect myRect = new Rect(0, 0, masterCanva.Width, masterCanva.Height);
            drawingContext.DrawRectangle(mySolidColorBrush, myPen, myRect);
            //drawingContext.DrawLine();
            if (_sourceImage == null)
            {
                return;
            }
            drawingContext.DrawImage(_sourceImage,_displayArea);
            var itemSize = new System.Windows.Size(_sourceImage.Width, _sourceImage.Height);
            foreach (var shape in Shapes)
            {
                shape.Paint(drawingContext,_displayArea,itemSize);
            }

            foreach (var obj in _paintData.PaintObjects)
            {
                obj.Paint(drawingContext,_displayArea,itemSize);
            }
        }

        public class ImageShapeDataChangedEventArgs : RoutedEventArgs
        {
            public ImageShapeDataChangedEventArgs(RoutedEvent routedEvent, object source, ImageShape imageShape) :
                base(routedEvent,source)
            {
                changedShape = imageShape;
            }
            public ImageShape changedShape { get;}
        }
        
        public delegate void ImageShapeDataChangedHandler(object sender ,ImageShapeDataChangedEventArgs e);
        
        public static readonly RoutedEvent ImageShapeDataChangedEvent = EventManager.RegisterRoutedEvent(
            "ImageShapeDataChanged", RoutingStrategy.Bubble, typeof(ImageShapeDataChangedHandler), typeof(ImageToolDisplayer));

        public event ImageShapeDataChangedHandler ImageShapeDataChanged
        {
            add { AddHandler(ImageShapeDataChangedEvent,value); }
            remove { RemoveHandler(ImageShapeDataChangedEvent,value); }
        }
        internal void ImageShapeDataChangedCallBack(ImageShape imageShape)
        {
            ImageShapeDataChangedEventArgs args = new ImageShapeDataChangedEventArgs(ImageShapeDataChangedEvent, this,imageShape);
            SetValue(ImageShapesProperty,ImageShapes);
            this.RaiseEvent(args);
        }
        private void ImageToolDisplayer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            masterCanva.Width = e.NewSize.Width;
            masterCanva.Height = e.NewSize.Height;
            Clip = new RectangleGeometry(new Rect(0, 0, masterCanva.Width, masterCanva.Height));
            ResetScale();
            InvalidateVisual();
        }
    }
}