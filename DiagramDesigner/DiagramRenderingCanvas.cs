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

        private readonly Pen previewLinePen = new Pen(Brushes.Gray, 1);
        private readonly Pen contextPen = new Pen(Brushes.Blue, 2);
        private readonly Pen additionPen = new Pen(Brushes.Red, 2);
        private readonly Pen normalPen = new Pen(Brushes.Black, 2);

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

		public void RenderVisual(List<List<WinPoint>> wallsToRender, 
            List<Tuple<int, int, int>> segmentsToHighlightAsContext,
            List<Tuple<int, int, int>> segmentsToHighlightAsAddition,
            (WinPoint startPoint, WinPoint endPoint) newEdgePreview, 
            List<ProgramToRender> programsToRender, double scaleBarUnitLength)
        {
            Random random = new Random(1);
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                this.DrawScaleBar(dc, scaleBarUnitLength);
                // draw walls
                for (int i = 0; i < wallsToRender.Count; i++)
                {
                    var points = wallsToRender[i];
                    for (int j = 0; j < points.Count - 1; j++)
                    {
                        var startPoint = new WinPoint((int)points[j].X, (int)points[j].Y);
                        var endPoint = new WinPoint((int)points[j + 1].X, (int)points[j + 1].Y);

                        Pen pen;
                        if (segmentsToHighlightAsContext.Contains(new Tuple<int, int, int>(i, j, j+1)))
						{
                            pen = this.contextPen;
						}
                        else if (segmentsToHighlightAsAddition.Contains(new Tuple<int, int, int>(i, j, j + 1)))
						{
                            pen = this.additionPen;
						}
                        else
						{
                            pen = this.normalPen;
                        }

                        dc.DrawLine(pen, startPoint, endPoint);
                    }
                }

                // draw the programs
                foreach (ProgramToRender ptr in programsToRender)
				{
                    var programColor = this.RandomProgramColor(random, 100);
                    this.DrawPolygonFill(dc, ptr.Perimeter, new SolidColorBrush(programColor));
                    foreach (List<Point> innerPerimeter in ptr.InnerPerimeters)
                    {
                        this.DrawPolygonFill(dc, innerPerimeter, new SolidColorBrush(Color.FromArgb(100, 255, 255, 0)));
                    }

                    String label = $@"{ptr.Name}
{Math.Floor(ptr.Area)}";
                    var labelColor = programColor;
                    labelColor.A = 255;
                    labelColor.R -= 80;
                    labelColor.G -= 80;
                    labelColor.B -= 80;
                    var formattedText = new FormattedText(label, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Arial"), 9, new SolidColorBrush(labelColor), VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    formattedText.TextAlignment = TextAlignment.Center;
                    var labelOrigin = ptr.GetVisualCenter();
                    dc.DrawText(formattedText, new WinPoint(labelOrigin.X, labelOrigin.Y - formattedText.Height / 2));
                }

                // draw the preview line
                dc.DrawLine(this.previewLinePen, newEdgePreview.startPoint, newEdgePreview.endPoint);
            }
        }

        private Color RandomProgramColor(Random r, int alpha)
		{
            return Color.FromArgb((byte)alpha, (byte)r.Next(100, 255), (byte)r.Next(100, 255), (byte)r.Next(100, 255));
		}

        private void DrawScaleBar(DrawingContext context, double scaleBarUnitLength)
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
                context.DrawText(formattedText, lineGoesUp ? new WinPoint(w - formattedText.Width / 2, h - ScaleBarHeight - formattedText.Height) : new WinPoint(w - formattedText.Width / 2, h - formattedText.Height));

                var p1 = new WinPoint(w, h);
                h = lineGoesUp ? h - ScaleBarHeight : h + ScaleBarHeight;
                var p2 = new WinPoint(w, h);
                context.DrawLine(ScaleBarPen, p1, p2);

                w -= scaleBarUnitLength * (vs[i] - vs[i + 1]);
                var p3 = new WinPoint(w, h);
                context.DrawLine(ScaleBarPen, p2, p3);

                lineGoesUp = !lineGoesUp;
            }
            formattedText = new FormattedText("0", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Arial"), 9, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            context.DrawText(formattedText, lineGoesUp ? new WinPoint(w - formattedText.Width / 2, h - ScaleBarHeight - formattedText.Height) : new WinPoint(w - formattedText.Width / 2, h - formattedText.Height));

            var p4 = new WinPoint(w, h);
            h = lineGoesUp ? h - ScaleBarHeight : h + ScaleBarHeight;
            var p5 = new WinPoint(w, h);
            context.DrawLine(ScaleBarPen, p4, p5);
        }

        private void DrawPolygonFill(DrawingContext context, List<WinPoint> points, Brush brush)
        {
            var start = points[0];
            List<WinLineSegment> segments = new List<WinLineSegment>();
            for (int i = 1; i < points.Count; i++)
            {
                segments.Add(new WinLineSegment(points[i], false));
            }
            var figure = new PathFigure(start, segments, true);
            var geo = new PathGeometry(new[] { figure });
            context.DrawGeometry(brush, null, geo);
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

