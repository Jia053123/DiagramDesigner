using BasicGeometries;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesignerGeometryParser
{
	public abstract class DiagramFragment
	{
		protected CycleOfLineSegments Perimeter = null;
		protected List<CycleOfLineSegments> InnerPerimeters = new List<CycleOfLineSegments>();
		protected List<LineSegment> SegmentsWithin = new List<LineSegment>();

		/// <summary>
		/// the list is in the order of connection and the first and last segments are connected
		/// </summary>
		public CycleOfLineSegments GetPerimeter()
		{
			return new CycleOfLineSegments(this.Perimeter.GetPerimeter());
		}

		/// <summary>
		/// the list is in the order of connection and the first and last points are the same
		/// </summary>
		public List<Point> GetPerimeterInPoints()
		{
			return this.ConvertPolyLineToEndpoints(this.GetPerimeter().GetPerimeter());
		}

		/// <summary>
		/// the list is in the order of connection and the first and last segments are connected
		/// </summary>
		public List<CycleOfLineSegments> GetInnerPerimeters()
		{
			var list = new List<CycleOfLineSegments>();
			foreach (CycleOfLineSegments cycle in this.InnerPerimeters)
			{
				list.Add(new CycleOfLineSegments(cycle.GetPerimeter()));
			}
			return list;
		}

		/// <summary>
		/// the list is in the order of connection and the first and last points are the same
		/// </summary>
		public List<List<Point>> GetInnerPerimetersInPoints()
		{
			var cycles = this.GetInnerPerimeters();
			var points = new List<List<Point>>();
			foreach (CycleOfLineSegments c in cycles)
			{
				points.Add(this.ConvertPolyLineToEndpoints(c.GetPerimeter()));
			}
			return points;
		}

		internal List<LineSegment> GetSegmentsWithin()
		{
			return new List<LineSegment>(this.SegmentsWithin);
		}

		internal bool IsLineSegmentInPerimeters(LineSegment ls)
		{
			// within the outer perimeter
			if (this.Perimeter.IsLineSegmentInCycle(ls))
			{
				// outside each inner perimeter
				foreach (CycleOfLineSegments innerPerimeter in this.InnerPerimeters)
				{
					if (innerPerimeter.IsLineSegmentInCycle(ls))
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		protected bool IsAnEndpointOnPerimeter(Point point)
		{
			foreach (LineSegment ls in this.Perimeter.GetPerimeter())
			{
				if (ls.FirstPoint == point || ls.SecondPoint == point)
				{
					return true;
				}
			}
			return false;
		}

		protected List<LineSegment> ConvertPointsToPolyline(List<Point> points)
		{
			List<LineSegment> polyline = new List<LineSegment>();
			for (int i = 1; i < points.Count; i++)
			{
				polyline.Add(new LineSegment(points[i - 1], points[i]));
			}
			return polyline;
		}

		protected List<Point> ConvertPolyLineToEndpoints(List<LineSegment> polyline)
		{
			// TODO: should I check segments are connected?

			List<Point> endpoints = new List<Point>();

			// add the first point
			var firstSegment = polyline[0];
			var secondSegment = polyline[1];

			bool towardsFirstPoint;
			if (firstSegment.FirstPoint == secondSegment.FirstPoint || firstSegment.FirstPoint == secondSegment.SecondPoint)
			{
				towardsFirstPoint = true;
				endpoints.Add(firstSegment.SecondPoint);
			}
			else
			{
				towardsFirstPoint = false;
				endpoints.Add(firstSegment.FirstPoint);
			}

			for (int i = 0; i < polyline.Count; i++)
			{
				var segment = polyline[i];
				endpoints.Add(towardsFirstPoint ? segment.FirstPoint : segment.SecondPoint);
				if (i < polyline.Count - 1)
				{
					// not at the end yet
					towardsFirstPoint = endpoints.Last() == polyline[i + 1].SecondPoint;
				}
			}

			return endpoints;
		}
	}



}
