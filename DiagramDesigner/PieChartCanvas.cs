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
    class PieChartCanvas : Canvas
    {
        private DataTable DataSource { get; set; }
        private String KeyColumnName;
        private String ValueColumnName;

        private double ChartRadius { get { return this.Width > this.Height ? this.Height / 2 * 0.8 : this.Width / 2 * 0.8; } }
        private WinPoint ChartCenter { get { return new WinPoint(this.Width / 2, this.Height / 2); } }
        private Pen PieChartPen { get { return new Pen(Brushes.Black, 1); } }

        private DrawingVisual sourceVisual = null;
        protected override int VisualChildrenCount { get { return 1; } }

        public PieChartCanvas()
        {
            this.Background = Brushes.Transparent;
            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }

        /// <summary>
        /// Render a data source with a string key column and a numeric value column into a pie chart
        /// </summary>
        /// <param name="source"> the source data table </param>
        /// <param name="keyColumnName"> the name of each numeric value </param>
        /// <param name="valueColumnName"> the numeric value corresponding to each key </param>
        public void RenderPieChart(DataTable source, String keyColumnName, String valueColumnName)
        {
            this.DataSource = source;
            this.KeyColumnName = keyColumnName;
            this.ValueColumnName = valueColumnName;

            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                // draw circle
                dc.DrawEllipse(null, this.PieChartPen, this.ChartCenter, this.ChartRadius, this.ChartRadius);

                // draw starting line
                dc.DrawLine(this.PieChartPen, this.ChartCenter, new WinPoint(this.ChartCenter.X + this.ChartRadius, this.ChartCenter.Y));

                // draw sections
                var testSum = this.DataSource.Compute("Sum(" + ValueColumnName + ")", null);
                if (! (testSum is double)) { return; }
                double sum = (double) testSum;

                double angle = 0;
                for (int i = 0; i < this.DataSource.Rows.Count; i++)
                {
                    // draw line
                    double value = this.DataSource.Rows[i].Field<double>(ValueColumnName);
                    double ratio = value / sum;
                    double angleDelta = 2 * Math.PI * ratio;

                    dc.DrawLine(this.PieChartPen,
                        this.ChartCenter,
                        new WinPoint(this.ChartCenter.X + this.ChartRadius * Math.Cos(angle+angleDelta), this.ChartCenter.Y - this.ChartRadius * Math.Sin(angle+angleDelta)));

                    // draw label
                    var labelText = $@"{this.DataSource.Rows[i].Field<String>(KeyColumnName)} 
{((int)(ratio * 100)).ToString()}%";

                    var formattedText = new FormattedText(labelText, 
                        CultureInfo.GetCultureInfo("en-us"), 
                        System.Windows.FlowDirection.LeftToRight, 
                        new Typeface("Arial"), 
                        11, 
                        Brushes.Black, 
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);

                    formattedText.TextAlignment = System.Windows.TextAlignment.Center;

                    dc.DrawText(formattedText,
                        new WinPoint(this.ChartCenter.X + (this.ChartRadius * Math.Cos(angle + angleDelta / 2))/2, 
                            this.ChartCenter.Y - (this.ChartRadius * Math.Sin(angle + angleDelta / 2))/2 - formattedText.Height / 2));

                    // offset angle starting point
                    angle += angleDelta;
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
    }
}
