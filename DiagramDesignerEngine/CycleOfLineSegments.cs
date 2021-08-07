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

		internal bool IsPointInCycle(Point point)
		{
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
			
			var rayCastingSegment = new LineSegment(leftMostPoint, point);
			var numOfIntersection = 0;
			foreach (LineSegment ls in this.Cycle)
			{
				if (!(rayCastingSegment.FindIntersection(ls) is null)) {
					numOfIntersection++;
				}
			}

			return !(numOfIntersection % 2 == 0); // if even, then outside
		}
	}
}
