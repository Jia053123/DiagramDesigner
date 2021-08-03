using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
	/// <summary>
	/// A straight line segment defined by its two end points. The two end points cannot be the same point. 
	/// </summary>
	class LineSegment
	{
		public Point EndPoint1 { get; private set; }
		public Point EndPoint2 { get; private set; }

		public LineSegment(Point endPoint1, Point endPoint2)
		{
			if (endPoint1 is null || endPoint2 is null || endPoint1 == endPoint2)
			{
				throw new ArgumentException("parameters cannot be null or identical");
			}

			this.EndPoint1 = endPoint1;
			this.EndPoint2 = endPoint2;
		}
	}
}
