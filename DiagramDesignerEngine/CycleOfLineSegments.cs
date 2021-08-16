using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesignerEngine
{
	class CycleOfLineSegments
	{
		// the list is in the order of connection and the first and last segment are connected
		private List<LineSegment> Cycle = null;

		/// <summary>
		/// return the perimeter segments in the order of connection and the first and last segments are connected
		/// </summary>
		internal List<LineSegment> GetPerimeter()
		{
			// make a copy since LineSegment is value type
			return new List<LineSegment>(Cycle);
		}

		/// <summary>
		/// A cycle consists of at least 3 segments. All segments are connected end-to-end to form one perfect loop
		/// </summary>
		/// <param name="segmentsFormingCycle"> must contain at least 3 elements and no redundant segments </param>
		internal CycleOfLineSegments(List<LineSegment> segmentsFormingCycle)
		{
			if (segmentsFormingCycle.Count < 3)
			{
				throw new ArgumentException("input has less than 3 elements");
			}

			if (segmentsFormingCycle.Distinct().ToList().Count < segmentsFormingCycle.Count)
			{
				throw new ArgumentException("input contains duplicate");
			}

			var traverser = new LineSegmentsTraverser(segmentsFormingCycle);
			var result = traverser.TraverseSegments(segmentsFormingCycle.First(), true, true); // doesn't matter what the parameters are since it's supposed to be a perfect loop
			if (result.Item1 != 0 || traverser.GetLastPath().Count != segmentsFormingCycle.Count)
			{
				throw new ArgumentException("does not form a perfect loop");
			}

			this.Cycle = traverser.GetLastPath();
		}

		/// <summary>
		/// Whether a line segments lies within the cycle (not if overlaping with the perimeter), 
		/// assuming the geometry is in exploded state
		/// </summary>
		/// <param name="lineSegment"> the line segment to check against the cycle </param>
		/// <returns> return true if and only if a line segment is within the cycle and not overlapping with the cycle </returns>
		internal bool IsLineSegmentInCycle(LineSegment lineSegment)
		{
			foreach (LineSegment pls in this.GetPerimeter())
			{
				if (LineSegment.DoOverlap(pls, lineSegment))
				{
					return false;
				}
			}

			var middlePoint = new Point((lineSegment.FirstPoint.coordinateX + lineSegment.SecondPoint.coordinateX) / 2.0,
				(lineSegment.FirstPoint.coordinateY + lineSegment.SecondPoint.coordinateY) / 2.0);

			return (this.IsPointInCycle(lineSegment.FirstPoint) && 
				this.IsPointInCycle(lineSegment.SecondPoint) && 
				this.IsPointInCycle(middlePoint));
		}

		/// <summary>
		/// Whether a point is in the cycle (being on the perimeter also counts)
		/// </summary>
		/// <param name="point"> the point to check against the cycle </param>
		/// <returns> true if the point is within or on the boundary of the cycle </returns>
		private bool IsPointInCycle(Point point)
		{
			// if the point is on the cycle, return true
			foreach (LineSegment ls in this.Cycle)
			{
				if (ls.ContainsPoint(point))
				{
					return true;
				}
			}

			// find the left most point on the cycle
			var leftMostPoint = this.Cycle.First().FirstPoint;
			foreach (LineSegment ls in this.Cycle)
			{
				if (ls.FirstPoint.coordinateX < leftMostPoint.coordinateX)
				{
					leftMostPoint = ls.FirstPoint;
				}
				else if (ls.SecondPoint.coordinateX < leftMostPoint.coordinateX) 
				{
					leftMostPoint = ls.SecondPoint;
				}
			}
			// use Ray Casting algorithm to determine if it's inside
			var rayCastingSegment = new LineSegment(new Point(leftMostPoint.coordinateX - 10, point.coordinateY), point);
			// count the number of unique intersection points instead of the number of intersection itself
			HashSet<Point> intersectionPoints = new HashSet<Point>();
			foreach (LineSegment ls in this.Cycle)
			{
				var ip = rayCastingSegment.FindIntersection(ls);
				if (! (ip is null))
				{
					intersectionPoints.Add((Point)ip);
				}
			}

			return !(intersectionPoints.Count % 2 == 0); // if even, then outside
		}
	}
}
