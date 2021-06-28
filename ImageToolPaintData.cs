using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using OpenCvSharp;

namespace ImageToolWPF
{
    public class ImageToolPaintData
    {
        public ImageToolPaintData ()
        {
            _paintObjects = new List<PaintObject>();
        }
        public void DrawMat(ref Mat paintMat)
        {
            foreach (var obj in PaintObjects)
            {
                obj.DrawMat(ref paintMat);
            }
        }
        private List<PaintObject> _paintObjects;
        public List<PaintObject> PaintObjects
        {
            get
            {
                return _paintObjects;
            }
            set
            {
                _paintObjects = value;
            }
        }
    }
}