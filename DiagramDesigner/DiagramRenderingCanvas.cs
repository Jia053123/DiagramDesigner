using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiagramDesignerEngine;
using WinPoint = System.Windows.Point;
using Point = DiagramDesignerEngine.Point;

namespace DiagramDesigner
{
    partial class DiagramRenderingCanvas: Canvas
    {
        private DrawingVisual sourceVisual = null;  
        
        protected override int VisualChildrenCount { get { return 1; } }

        public event EventHandler MouseMovedEventHandler;
        public event EventHandler MouseLeftClickedEventHandler;

        public DiagramRenderingCanvas()
        {
            this.Background = Brushes.White;
            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }

		public void RenderVisual(List<List<Point>> polylinesToRender, (Point startPoint, Point endPoint) newEdgePreview)
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                // draw polyline
                for (int i = 0; i < polylinesToRender.Count; i++)
                {
                    for (int j = 0; j < polylinesToRender[i].Count-1; j++)
					{
                        var startPoint = new WinPoint((int)polylinesToRender[i][j].coordinateX, (int)polylinesToRender[i][j].coordinateY);
                        var endPoint = new WinPoint((int)polylinesToRender[i][j+1].coordinateX, (int)polylinesToRender[i][j+1].coordinateY);
                        dc.DrawLine(new Pen(Brushes.Black, 1), startPoint, endPoint);
                    }
                }
                // draw the preview line
                dc.DrawLine(new Pen(Brushes.Blue, 1), 
                    new WinPoint(newEdgePreview.startPoint.coordinateX, newEdgePreview.startPoint.coordinateY), 
                    new WinPoint(newEdgePreview.endPoint.coordinateX, newEdgePreview.endPoint.coordinateY));
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return sourceVisual;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            WinPoint location = e.GetPosition(this);

            if (this.MouseMovedEventHandler != null)
            {
                this.MouseMovedEventHandler.Invoke(this, new MouseEventArgs(location.X, location.Y));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            WinPoint location = e.GetPosition(this);

            if (this.MouseLeftClickedEventHandler != null)
            {
                this.MouseLeftClickedEventHandler.Invoke(this, new MouseEventArgs(location.X, location.Y));
            }
        }
    }
}

