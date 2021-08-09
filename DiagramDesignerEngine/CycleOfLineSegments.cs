using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesignerEngine
{
	class CycleOfLineSegments
	{
		// the first and last segment must be connected
		private List<LineSegment> Cycle = new List<LineSegment>();

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
			if (segmentsFormingCycle.Count < 3 || (segmentsFormingCycle.Distinct().ToList().Count < segmentsFormingCycle.Count))
			{
				throw new ArgumentException("input has less than 3 elements or contains duplicates");
			}

			var traverser = new LineSegmentsTraverser(segmentsFormingCycle);
			var result = traverser.TraverseSegments(segmentsFormingCycle.First(), true, true); // doesn't matter what the parameters are since it's supposed to be a perfect loop
			if (result != 0 || traverser.GetPath().Count != segmentsFormingCycle.Count)
			{
				throw new ArgumentException("does not form a perfect loop");
			}

			this.Cycle = segmentsFormingCycle;
		}

		/// <summary>
		/// Whether a line segments lies within the cycle
		/// </summary>
		/// <param name="lineSegment"> the line segment to check against the cycle </param>
		/// <returns> true if and only if both end points of the segment are either on or within the cycle perimeter </returns>
		internal bool IsLineSegmentInCycle(LineSegment lineSegment)
		{
			return (this.IsPointInCycle(lineSegment.FirstPoint) && this.IsPointInCycle(lineSegment.SecondPoint));
		}

		/// <summary>
		/// Whether a point is in or on the cycle
		/// </summary>
		/// <param name="point"> the point to check against the cycle </param>
		/// <returns> true if the point is within or on the boundary of the cycle </returns>
		private bool IsPointInCycle(Point point)
		{
			// if the point is on the cycle, return true
			foreach (LineSegment ls in this.Cycle)
			{
				if (ls.FirstPoint == point || ls.SecondPoint == point)
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
				else if (ls.SecondPoint.coordinateX < leftMostPoint.coordinateX) // probably unnecessary because they are connected
				{
					leftMostPoint = ls.SecondPoint;
				}
			}
			// use Ray Casting algorithm to determine if it's inside
			var rayCastingSegment = new LineSegment(leftMostPoint, point);
			var numOfTimesPerimeterPassed = 0;
			foreach (LineSegment ls in this.Cycle)
			{
				if (!(rayCastingSegment.FindIntersection(ls) is null))
				{
					numOfTimesPerimeterPassed++;
				}
			}

			return !(numOfTimesPerimeterPassed % 2 == 0); // if even, then outside
		}
	}
}
