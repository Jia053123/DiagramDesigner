using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;
using Point = DiagramDesignerEngine.Point;

namespace DiagramDesigner
{
	static class Utilities
	{
		public static WinPoint ConvertToSystemPoint(Point p)
		{
			return new WinPoint(p.coordinateX, p.coordinateY);
		}
	}
}
