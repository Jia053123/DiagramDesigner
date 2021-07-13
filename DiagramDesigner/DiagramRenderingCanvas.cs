using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace DiagramDesigner
{
    class DiagramRenderingCanvas: Canvas 
    {
        private DrawingVisual sourceVisual = null;

        public DiagramRenderingCanvas()
        {
            sourceVisual = new DrawingVisual();

            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                //TODO: draw!
                dc.DrawRectangle(null, new Pen(Brushes.Black, 1), new Rect(10,10,200,300));
            }

            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return sourceVisual;
        }
    }
}
