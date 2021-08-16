using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WinPoint = System.Windows.Point;
using WinLineSegment = System.Windows.Media.LineSegment;
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

        private readonly Brush BackgroundBrush = Brushes.White;

        public DiagramRenderingCanvas()
        {
            this.Background = this.BackgroundBrush;
            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }

		public void RenderVisual(List<List<WinPoint>> wallsToRender, (WinPoint startPoint, WinPoint endPoint) newEdgePreview, List<ProgramToRender> programsToRender, double scaleBarUnitLength)
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                void drawPolyline(List<WinPoint> points, Pen pen)
				{
                    for (int i = 0; i < points.Count-1; i++)
                    {
						var startPoint = new WinPoint((int)points[i].X, (int)points[i].Y);
						var endPoint = new WinPoint((int)points[i + 1].X, (int)points[i + 1].Y);
						dc.DrawLine(pen, startPoint, endPoint);
					}
				}

                void drawPolygonFill(List<WinPoint> points, Brush brush)
				{
                    var start = points[0];
                    List<WinLineSegment> segments = new List<WinLineSegment>();
                    for (int i = 1; i < points.Count; i++)
					{
                        segments.Add(new WinLineSegment(points[i], false));
					}
                    var figure = new PathFigure(start, segments, true);
                    var geo = new PathGeometry(new[] { figure });
                    dc.DrawGeometry(brush, null, geo);
				}

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

                // draw walls
                for (int i = 0; i < wallsToRender.Count; i++)
                {
                    drawPolyline(wallsToRender[i], new Pen(Brushes.Black, 1));
                }

                // draw the programs
                foreach (ProgramToRender ptr in programsToRender)
				{
                    drawPolygonFill(ptr.Perimeter, new SolidColorBrush(Color.FromArgb(100,255,0,0)));
                    foreach (List<Point> innerPerimeter in ptr.InnerPerimeters)
					{
                        drawPolygonFill(innerPerimeter, Brushes.Yellow);
					}
				}

                // draw the preview line
                dc.DrawLine(new Pen(Brushes.Blue, 1), newEdgePreview.startPoint, newEdgePreview.endPoint);
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
            if (!this.IsLocationInside(location.X, location.Y))
            {
                return;
            }
            if (this.MouseMovedEventHandler != null)
            {
                this.MouseMovedEventHandler.Invoke(this, new MouseEventArgs(location.X, location.Y));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            WinPoint location = e.GetPosition(this);
            if (! this.IsLocationInside(location.X, location.Y))
			{
                return;
			}
            if (this.MouseLeftClickedEventHandler != null)
            {
                this.MouseLeftClickedEventHandler.Invoke(this, new MouseEventArgs(location.X, location.Y));
            }
        }

        private bool IsLocationInside(double xLoc, double yLoc)
		{
            return (xLoc > 0 && xLoc < this.ActualWidth && yLoc > 0 && yLoc < this.ActualHeight);
		}
    }
}

