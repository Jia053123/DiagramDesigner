using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiagramDesignerEngine;

namespace DiagramDesigner
{
    class DiagramRenderingCanvas: Canvas 
    {
        private DrawingVisual sourceVisual = null;

        public DiagramRenderingCanvas()
        {
            this.Background = Brushes.White;
            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }

        public void RenderVisual(List<Point> pointsToRender)
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                //TODO: draw!
                for (int i = 0; i < pointsToRender.Count-1; i++)
                {
                    var startPoint = new System.Windows.Point((int)pointsToRender[i].coordinateX, (int)pointsToRender[i].coordinateY);
                    var endPoint = new System.Windows.Point((int)pointsToRender[i + 1].coordinateX, (int)pointsToRender[i + 1].coordinateY);
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            System.Windows.Point location = e.GetPosition(this);

            System.Diagnostics.Debug.WriteLine("mouse at location: {0}, {1}", location.X, location.Y);
        }
    }
}
