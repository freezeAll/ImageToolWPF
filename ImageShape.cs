using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using OpenCvSharp;
using Point = System.Windows.Point;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace ImageToolWPF
{
    [Serializable]
    public class ImageShape
    {
        internal ImageToolDisplayer _parentDisplayer;
        public string Name { get;set;}
        public string TypeName { get; set; }

        internal static double DotProduct(System.Windows.Vector v1, System.Windows.Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
        internal static double GetAnglePI(System.Windows.Vector v1, System.Windows.Vector v2)
        {
            double a = DotProduct(v1, v2) / Math.Sqrt(DotProduct(v1, v1) * DotProduct(v2, v2));
            a =Math.Min(1.0, Math.Max(-1.0, a));
            double t = Math.Acos(a);
            if (v1.X * v2.Y - v1.Y * v2.X < 0) t = -t;
            return t;
        }
        internal static Point MapToPaint(System.Windows.Point source, System.Windows.Rect imgRect, System.Windows.Size itemSize)
        {
            double power;
            if (itemSize.Width / itemSize.Height > imgRect.Width / imgRect.Height)
            {
                power = imgRect.Width / itemSize.Width ;
            }
            else
            {
                power =  imgRect.Height / itemSize.Height;
            }
            var outX = source.X * power + imgRect.X;
            var outY = source.Y * power + imgRect.Y;
            return new Point(outX,outY);
        }

        internal static double GetDistance(System.Windows.Point p1, System.Windows.Point p2)
        {
            var vec = p1 - p2;
            return Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        }
        internal static double GetDistance(System.Windows.Point p, System.Windows.Point p1,System.Windows.Point p2)
        {
            var a = p1 - p;
            var b = p2 - p;
            double r = (a.X - b.X) * (a.X) + (a.Y - b.Y) * a.Y;
            double d = (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
            if (r <= 0)
            {
                return a.X * a.X + a.Y * a.Y;
            }
            else if(r >= d)
            {
                return b.X * b.X + b.Y * b.Y;
            }
            r /= d;
            double x = a.X + (b.X - a.X) * r, y = a.Y + (b.Y - a.Y) * r;
            return x * x + y * y;
        }
        internal static Size MapToPaint(System.Windows.Size source, System.Windows.Rect imgRect, System.Windows.Size itemSize)
        {
            double power;
            if (itemSize.Width / itemSize.Height > imgRect.Width / imgRect.Height)
            {
                power = imgRect.Width / itemSize.Width ;
            }
            else
            {
                power =  imgRect.Height / itemSize.Height;
            }
            var outW = source.Width * power;
            var outH = source.Height * power;
            return new Size(outW,outH);
        }
        internal static Point MapToSource(System.Windows.Point mouse, System.Windows.Rect imgRect, System.Windows.Size itemSize)
        {
            double power;
            if (itemSize.Width / itemSize.Height > imgRect.Width / imgRect.Height)
            {
                power =itemSize.Width / imgRect.Width;
            }
            else
            {
                power =  itemSize.Height / imgRect.Height;
            }
            var outX = (mouse.X - imgRect.X) * power;
            var outY = (mouse.Y - imgRect.Y) * power ;
            return new Point(outX,outY);
        }
        internal static Size MapToSource(System.Windows.Size paint, System.Windows.Rect imgRect, System.Windows.Size itemSize)
        {
            double power;
            if (itemSize.Width / itemSize.Height > imgRect.Width / imgRect.Height)
            {
                power =itemSize.Width / imgRect.Width;
            }
            else
            {
                power =  itemSize.Height / imgRect.Height;
            }
            var outW = paint.Width  * power;
            var outH = paint.Height * power ;
            return new Size(outW,outH);
        }
        internal virtual bool UnderMouse(System.Windows.Point mouse,System.Windows.Rect imgRect,System.Windows.Size itemSize)
        {
            return false;
        }
        internal virtual void Paint(DrawingContext painter,System.Windows.Rect rect,System.Windows.Size itemSize)
        {
            
        }
        internal virtual void MouseMove(System.Windows.Point mouse,System.Windows.Rect imgRect,System.Windows.Size itemSize)
        {
            
        }
        internal virtual void HoverMove(System.Windows.Point mouse,System.Windows.Rect imgRect,System.Windows.Size itemSize)
        {
            
        }
    }

    
}