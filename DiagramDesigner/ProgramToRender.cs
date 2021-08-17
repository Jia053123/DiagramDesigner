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
            int pointCount = this.Perimeter.Count;

            foreach (WinPoint wp in this.Perimeter)
			{
                xTotal += wp.X;
                yTotal += wp.Y;
			}

            return new WinPoint(xTotal / pointCount, yTotal / pointCount);
		}
    }
}
