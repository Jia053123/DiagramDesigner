using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;
using DiagramDesignerEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("DiagramDesigner.UnitTests")]
namespace DiagramDesigner
{
	static class Utilities
	{
		public static WinPoint ConvertPointToWindowsPoint(Point p, double winPointUnitOverPointUnit)
		{
			return new WinPoint(p.coordinateX * winPointUnitOverPointUnit, p.coordinateY * winPointUnitOverPointUnit);
		}

		public static  Point ConvertWindowsPointToPoint (WinPoint wp, double winPointUnitOverPointUnit)
		{
			return new Point(wp.X / winPointUnitOverPointUnit, wp.Y / winPointUnitOverPointUnit);
		}

		public static double DistanceBetweenWinPoints(WinPoint p1, WinPoint p2)
		{
			var diffX = p1.X - p2.X;
			var diffY = p1.Y - p2.Y;
			return Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
		}
	}
}
