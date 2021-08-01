using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesignerEngine
{
    public class Point
    {
        public double coordinateX { get; set; }
        public double coordinateY { get; set; }

        public Point(double x, double y)
        {
            this.coordinateX = x;
            this.coordinateY = y;
        }

        public static explicit operator WinPoint (Point p)
		{
            return new WinPoint(p.coordinateX, p.coordinateY);
		}

        public static explicit operator Point (WinPoint wp)
		{
            return new Point(wp.X, wp.Y);
		}
    }
}
