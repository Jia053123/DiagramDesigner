using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
    internal readonly struct ProgramToRender
    {
        internal readonly List<WinPoint> Perimeter;
        internal readonly List<List<WinPoint>> InnerPerimeters;
        internal readonly String Name;
        internal readonly double Area;

        internal ProgramToRender(List<WinPoint> perimeter, List<List<WinPoint>> innerPerimeters, String name, double area)
        {
            this.Perimeter = perimeter;
            this.InnerPerimeters = innerPerimeters;
            this.Name = name;
            this.Area = area;
        }

        internal WinPoint GetCenterOfThePerimeter()
		{
            double xTotal = 0;
            double yTotal = 0;
            int pointCount = this.Perimeter.Count - 1;

            for (int i = 0; i < pointCount; i++) // the last point is repeated, so do not count
			{
                var wp = this.Perimeter[i];
                xTotal += wp.X;
                yTotal += wp.Y;
			}

            return new WinPoint(xTotal / pointCount, yTotal / pointCount);
		}
    }
}
