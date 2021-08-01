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
using System.Globalization;

namespace DiagramDesigner
{
    partial class DiagramRenderingCanvas: Canvas
    {
        private DrawingVisual sourceVisual = null;  
        
        protected override int VisualChildrenCount { get { return 1; } }

        public event EventHandler MouseMovedEventHandler;
        public event EventHandler MouseLeftClickedEventHandler;

        private readonly Pen ScaleBarPen = new Pen(Brushes.Black, 1);
        private readonly double ScaleBarHeight = 5;
        private readonly double ScaleBarPadding = 10;

        public DiagramRenderingCanvas()
        {
            this.Background = Brushes.White;
            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }

		public void RenderVisual(List<List<Point>> polylinesToRender, (Point startPoint, Point endPoint) newEdgePreview, double scaleBarUnitLength)
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                // draw scale bar
                FormattedText formattedText;
                var w = this.ActualWidth - ScaleBarPadding;
                var h = this.ActualHeight - ScaleBarPadding;
                var lineGoesUp = true;
                double[] vs = { 40, 20, 10, 5, 0 };
                for (int i = 0; i < 4; i++)
				{
                    var segmentLabel = ((int)vs[i]).ToString();
                    formattedText = new FormattedText(segmentLabel, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Arial"), 9, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    dc.DrawText(formattedText, lineGoesUp ? new WinPoint(w - formattedText.Width/2, h - ScaleBarHeight - formattedText.Height) : new WinPoint(w - formattedText.Width/2, h - formattedText.Height));

                    var p1 = new WinPoint(w, h);
                    h = lineGoesUp ? h - ScaleBarHeight : h + ScaleBarHeight;
                    var p2 = new WinPoint(w, h);
                    dc.DrawLine(ScaleBarPen, p1, p2);

                    w -= scaleBarUnitLength * vs[i];
                    var p3 = new WinPoint(w, h);
                    dc.DrawLine(ScaleBarPen, p2, p3);
                    
                    lineGoesUp = !lineGoesUp;
                }
                formattedText = new FormattedText("0", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Arial"), 9, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                dc.DrawText(formattedText, lineGoesUp ? new WinPoint(w - formattedText.Width/2, h - ScaleBarHeight - formattedText.Height) : new WinPoint(w - formattedText.Width/2, h - formattedText.Height));

                var p4 = new WinPoint(w, h);
                h = lineGoesUp ? h - ScaleBarHeight : h + ScaleBarHeight;
                var p5 = new WinPoint(w, h);
                dc.DrawLine(ScaleBarPen, p4, p5);

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

