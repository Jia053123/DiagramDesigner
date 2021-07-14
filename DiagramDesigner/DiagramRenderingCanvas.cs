using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace DiagramDesigner
{
    class DiagramRenderingCanvas: Canvas 
    {
        private DrawingVisual sourceVisual = null;

        public DiagramRenderingCanvas()
        {
            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }

        public void RenderVisual(List<DDPoint> pointsToRender)
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                //TODO: draw!
                for (int i = 0; i < pointsToRender.Count-1; i++)
                {
                    var startPoint = new Point((int)pointsToRender[i].coordinateX, (int)pointsToRender[i].coordinateY);
                    var endPoint = new Point((int)pointsToRender[i+1].coordinateX, (int)pointsToRender[i+1].coordinateY);
                    dc.DrawLine(new Pen(Brushes.Black, 1), startPoint, endPoint);
                }
            }
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
