using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
    class PieChart : Canvas
    {
        private DataTable DataSource { get; set; }
        private String KeyColumnName;
        private String ValueColumnName;
        private double ChartRadius { get { return this.Width > this.Height ? this.Height / 2 : this.Width / 2; } }
        private const double PenWidth = 1;
        private WinPoint ChartCenter { get { return new System.Windows.Point(this.Width / 2, this.Height / 2); } }

        private DrawingVisual sourceVisual = null;

        public PieChart(DataTable source, String keyColumnName, String valueColumnName)
        {
            this.DataSource = source;
            this.KeyColumnName = keyColumnName;
            this.ValueColumnName = valueColumnName;

            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);

            this.RenderPieChart();
        }

        public void RenderPieChart()
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                // draw circle
                dc.DrawEllipse(null, new Pen(Brushes.Black, PieChart.PenWidth), this.ChartCenter, this.ChartRadius, this.ChartRadius);

                // draw sections
                var sum = (double)this.DataSource.Compute("Sum(ValueName)", "");

                dc.DrawLine(new Pen(Brushes.Black, PieChart.PenWidth), 
                    this.ChartCenter, 
                    new WinPoint(this.ChartCenter.X, this.ChartCenter.Y + this.ChartRadius));

                double angle = 0;
                for (int i = 0; i < this.DataSource.Rows.Count; i++)
                {
                    // draw line
                    var value = this.DataSource.Rows[i].Field<double>(ValueColumnName);
                    var ratio = value / sum;
                    var angleDelta = 2 * Math.PI * ratio;
                    dc.DrawLine(new Pen(Brushes.Black, PieChart.PenWidth),
                        this.ChartCenter,
                        new WinPoint(this.ChartCenter.X + this.ChartRadius * Math.Cos(angle+angleDelta), this.ChartCenter.Y + this.ChartRadius * Math.Sin(angle+angleDelta)));

                    // draw label
                    var labelText = this.DataSource.Rows[i].Field<String>(KeyColumnName) + (ratio*100).ToString() + "%";

                    var formattedText = new FormattedText(labelText, 
                        CultureInfo.GetCultureInfo("en-us"), 
                        System.Windows.FlowDirection.LeftToRight, 
                        new Typeface("Arial"), 
                        11, 
                        Brushes.Black, 
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);

                    dc.DrawText(formattedText,
                        new WinPoint(this.ChartCenter.X + (this.ChartRadius * Math.Cos(angle + angleDelta / 2))/2, this.ChartCenter.Y + (this.ChartRadius * Math.Sin(angle + angleDelta / 2))/2));

                    // offset angle starting point
                    angle += angleDelta;
                }
            }
        }
    }
}
