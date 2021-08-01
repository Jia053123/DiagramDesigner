using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;
using DiagramDesignerEngine;

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
	}
}
