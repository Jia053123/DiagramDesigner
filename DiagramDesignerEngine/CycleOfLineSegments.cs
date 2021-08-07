using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class CycleOfLineSegments
	{
		// the first and last segment must be connected
		private List<LineSegment> Cycle = new List<LineSegment>();

		/// <summary>
		/// A cycle consists of at least 3 segments. All segments are connected end-to-end to form a loop
		/// </summary>
		/// <param name="segments"> must contain at least 3 elements and no extra segments except for dangling ones </param>
		internal CycleOfLineSegments(List<LineSegment> segments)
		{
			if (segments.Count < 3 || (segments.Distinct().ToList().Count < segments.Count))
			{
				throw new ArgumentException("input has less than 3 elements or contains duplicates");
			}

			// remove dangling segments
			foreach (LineSegment ls in segments)
			{
				var leftResult = GeometryUtilities.FindLeftConnectedSegmentsSortedByAngle(ls, segments);
				var rightResult = GeometryUtilities.FindRightConnectedSegmentsSortedByAngle(ls, segments);

				if (leftResult.Count == 0 || rightResult.Count == 0)
				{
					segments.Remove(ls);
				}
			}

			// verify the cycle
			var currentSegment = segments[0];
			bool IsFirstPointTheOneToSearch = true;
			do
			{
				List<LineSegment> searchResult;
				if (IsFirstPointTheOneToSearch)
				{
					searchResult = GeometryUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, segments);
				}
				else
				{
					searchResult = GeometryUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, segments);
				}
				if (searchResult.Count != 1)
				{
					throw new ArgumentException("not all segments are connected at both ends or there are branches");
				}

				var nextSegment = searchResult.First();
				if (IsFirstPointTheOneToSearch)
				{
					IsFirstPointTheOneToSearch = currentSegment.FirstPoint != nextSegment.FirstPoint; // if not the first point, must be equal to the second point then
				}
				else
				{
					IsFirstPointTheOneToSearch = currentSegment.SecondPoint != nextSegment.FirstPoint;
				}
				Cycle.Add(currentSegment);
				currentSegment = nextSegment;
			} while (Cycle.Count < segments.Count);

			if (segments.Count < 3)
			{
				throw new ArgumentException("cannot find a loop with more than 2 elements ");
			}

			// check the front and end of the loop are connected
			if (GeometryUtilities.FindLeftConnectedSegmentsSortedByAngle(Cycle.First(), Cycle).Count != 1 ||
				GeometryUtilities.FindRightConnectedSegmentsSortedByAngle(Cycle.First(), Cycle).Count != 1)
			{
				throw new ArgumentException("not all segments are connected at both ends or there are branches");
			}
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
		internal bool IsPointInCycle(Point point)
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
