using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiagramDesignerEngine;
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

            System.Windows.Point location = e.GetPosition(this);

            var boundedLocation = this.BoundCursorPositionWithinControl(location);

            if (this.MouseMovedEventHandler != null)
            {
                this.MouseMovedEventHandler.Invoke(this, new MouseEventArgs(boundedLocation.X, boundedLocation.Y));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            System.Windows.Point location = e.GetPosition(this);

            var boundedLocation = this.BoundCursorPositionWithinControl(location);

            if (this.MouseLeftClickedEventHandler != null)
            {
                this.MouseLeftClickedEventHandler.Invoke(this, new MouseEventArgs(boundedLocation.X, boundedLocation.Y));
            }
        }

        private System.Windows.Point BoundCursorPositionWithinControl(System.Windows.Point location)
        {
            var locationWithinBound = new System.Windows.Point(location.X, location.Y);

            if (locationWithinBound.X < 0)
            {
                locationWithinBound.X = 0;
            }
            else if (locationWithinBound.X > this.Width)
            {
                locationWithinBound.X = this.Width;
            }

            if (locationWithinBound.Y < 0)
            {
                locationWithinBound.Y = 0;
            }
            else if (locationWithinBound.Y > this.Height)
            {
                locationWithinBound.Y = this.Height;
            }

            return locationWithinBound;
        }
    }
}

