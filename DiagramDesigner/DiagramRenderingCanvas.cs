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
        private List<DDPoint> pointsToRender = new List<DDPoint> { new DDPoint(10, 10), new DDPoint(20, 30), new DDPoint(50, 45), new DDPoint(100, 100) }; // TODO: stub

        public DiagramRenderingCanvas()
        {
            sourceVisual = new DrawingVisual();

            this.RenderVisual();

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

        public void RenderVisual()
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                //TODO: draw!
                for (int i = 0; i < this.pointsToRender.Count-1; i++)
                {
                    var startPoint = new Point((int)this.pointsToRender[i].coordinateX, (int)this.pointsToRender[i].coordinateY);
                    var endPoint = new Point((int)this.pointsToRender[i+1].coordinateX, (int)this.pointsToRender[i+1].coordinateY);
                    dc.DrawLine(new Pen(Brushes.Black, 1), startPoint, endPoint);
                }
            }
        }
    }
}
