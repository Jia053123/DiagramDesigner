using System;
using System.Collections.Generic;
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

		internal WinPoint GetVisualCenter()
		{
			WinPoint centroid = new WinPoint(0, 0);
			double signedArea = 0.0;
			double x0 = 0.0; // Current vertex X
			double y0 = 0.0; // Current vertex Y
			double x1 = 0.0; // Next vertex X
			double y1 = 0.0; // Next vertex Y
			double a = 0.0;  // Partial signed area

			for (int i = 0; i < this.Perimeter.Count; i++)
			{
				x0 = this.Perimeter[i].X;
				y0 = this.Perimeter[i].Y;
				x1 = this.Perimeter[(i + 1) % this.Perimeter.Count].X;
				y1 = this.Perimeter[(i + 1) % this.Perimeter.Count].Y;
				a = x0 * y1 - x1 * y0;
				signedArea += a;
				centroid.X += (x0 + x1) * a;
				centroid.Y += (y0 + y1) * a;
			}

			signedArea *= 0.5;
			centroid.X /= (6.0 * signedArea);
			centroid.Y /= (6.0 * signedArea);

			return centroid;
		}
	}
}
