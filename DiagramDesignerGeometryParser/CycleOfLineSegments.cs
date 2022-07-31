using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesignerGeometryParser
{
	public class CycleOfLineSegments
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
			var result = traverser.TraverseSegments(segmentsFormingCycle.First(), true, true); // the last parameter probably doesn't matter
			if (result.Item1 != 0 || traverser.GetLastPath().Count != segmentsFormingCycle.Count)
			{
				throw new ArgumentException("does not form a perfect loop");
			}

			// no need to check overlapping endpoints because if that happens either the result won't be 0 or the traverser won't cover all segments

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
				if (ls.RoughlyContainsPoint(point))
				{
					return true;
				}
			}

			// find the left most point on the cycle
			var leftMostPoint = this.Cycle.First().FirstPoint;
			var rightMostPoint = this.Cycle.First().FirstPoint;
			var topMostPoint = this.Cycle.First().FirstPoint;
			var bottomMostPoint = this.Cycle.First().FirstPoint;
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

				if (ls.FirstPoint.coordinateX > rightMostPoint.coordinateX)
				{
					rightMostPoint = ls.FirstPoint;
				}
				else if (ls.SecondPoint.coordinateX > rightMostPoint.coordinateX)
				{
					rightMostPoint = ls.SecondPoint;
				}

				if (ls.FirstPoint.coordinateY < bottomMostPoint.coordinateY)
				{
					bottomMostPoint = ls.FirstPoint;
				}
				else if (ls.SecondPoint.coordinateY < bottomMostPoint.coordinateY)
				{
					bottomMostPoint = ls.SecondPoint;
				}

				if (ls.FirstPoint.coordinateY > topMostPoint.coordinateY)
				{
					topMostPoint = ls.FirstPoint;
				}
				else if (ls.SecondPoint.coordinateY > topMostPoint.coordinateY)
				{
					topMostPoint = ls.SecondPoint;
				}
			}
			// use Ray Casting algorithm to determine if it's inside: count the number of unique intersection points in four directions
			var rayCastingSegmentLeft = new LineSegment(new Point(leftMostPoint.coordinateX - 10, point.coordinateY), point);
			var rayCastingSegmentRight = new LineSegment(new Point(rightMostPoint.coordinateX + 10, point.coordinateY), point);
			var rayCastingSegmentBottom = new LineSegment(new Point(point.coordinateX, bottomMostPoint.coordinateY - 10), point);
			var rayCastingSegmentTop = new LineSegment(new Point(point.coordinateX, topMostPoint.coordinateY + 10), point);

			HashSet<Point> findUniqueIntersections(LineSegment rayCastingSegment)
			{
				HashSet<Point> intersectionPoints = new HashSet<Point>();
				foreach (LineSegment ls in this.Cycle)
				{
					var ip = rayCastingSegment.FindIntersection(ls);
					if (!(ip is null))
					{
						intersectionPoints.Add((Point)ip);
					}
					else if (LineSegment.DoOverlap(rayCastingSegment, ls))
					{
						// if they overlap, just for this method assume they intersect at the middle of ls
						intersectionPoints.Add(new Point((ls.FirstPoint.coordinateX + ls.SecondPoint.coordinateX) / 2.0,
							(ls.FirstPoint.coordinateY + ls.SecondPoint.coordinateY) / 2.0));
					}
				}
				return intersectionPoints;
			}

			var intersectionPointsLeft = findUniqueIntersections(rayCastingSegmentLeft);
			var intersectionPointsRight = findUniqueIntersections(rayCastingSegmentRight);
			var intersectionPointsBottom = findUniqueIntersections(rayCastingSegmentBottom);
			var intersectionPointsTop = findUniqueIntersections(rayCastingSegmentTop);

			// the point is inside if and only if all four results are odd
			return !(intersectionPointsLeft.Count % 2 == 0) &&
				!(intersectionPointsRight.Count % 2 == 0) &&
				!(intersectionPointsBottom.Count % 2 == 0) &&
				!(intersectionPointsTop.Count % 2 == 0);
		}
	}
}
