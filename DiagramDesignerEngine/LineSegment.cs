using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	/// <summary>
	/// A straight line segment defined by its two end points. The two end points cannot be the same point. 
	/// </summary>
	readonly struct LineSegment
	{
		public Point EndPoint1 { get; }
		public Point EndPoint2 { get; }

		internal LineSegment(Point endPoint1, Point endPoint2)
		{		
			if (endPoint1 == endPoint2)
			{
				throw new ArgumentException("Parameters cannot be identical");
			}

			this.EndPoint1 = endPoint1;
			this.EndPoint2 = endPoint2;
		}

		/// <summary>
		/// Line segments do not intersect if they overlap or merely share an endpoint. 
		/// They do intersect if they form a T shape. 
		/// </summary>
		/// <param name="ls"> A LineSegment to check for intersection </param>
		/// <returns> the point of intersection; null if not found </returns>
		internal Point? FindIntersection(LineSegment ls)
		{
			if (this.EndPoint1 == ls.EndPoint1 || this.EndPoint1 == ls.EndPoint2 || 
				this.EndPoint2 == ls.EndPoint1 || this.EndPoint2 == ls.EndPoint2)
			{
				// end points overlap
				return null;
			}

			var p1x = this.EndPoint1.coordinateX;
			var p1y = this.EndPoint1.coordinateY;
			var p2x = this.EndPoint2.coordinateX;
			var p2y = this.EndPoint2.coordinateY;

			var p3x = ls.EndPoint1.coordinateX;
			var p3y = ls.EndPoint1.coordinateY;
			var p4x = ls.EndPoint2.coordinateX;
			var p4y = ls.EndPoint2.coordinateY;

			var s1x = p2x - p1x;
			var s1y = p2y - p1y;
			var s2x = p4x - p3x;
			var s2y = p4y - p3y;

			if (s1x / s1y == s2x / s2y)
			{
				// slops are the same: they are parallel or overlap
				return null;
			}

			var s = (s1x * (p1y - p3y) - s1y * (p1x - p3x)) / (s1x * s2y - s2x * s1y);
			var t = (s2x * (p1y - p3y) - s2y * (p1x - p3x)) / (s1x * s2y - s2x * s1y);

			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
			{
				// Collision detected
				return new Point(p1x + (t * s1x), p1y + (t * s1y));
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Check whether the point is on the segment inbetween the two endpoints
		/// </summary>
		/// <param name="p"> The point to check </param>
		/// <returns> false if the point is not on the segment or is an endpoint </returns>
		internal bool ContainsPoint(Point p)
		{
			// check p is not one of the endpoints
			if (p == EndPoint1 || p == EndPoint2)
			{
				return false;
			}

			var crossProduct = (p.coordinateY - EndPoint1.coordinateY) * (EndPoint2.coordinateX - EndPoint1.coordinateX) -
				(p.coordinateX - EndPoint1.coordinateX) * (EndPoint2.coordinateY - EndPoint1.coordinateY);
			if (Math.Abs(crossProduct) > double.Epsilon)
			{
				// the three points are not aligned
				return false;
			}

			var dotProduct = (p.coordinateX - EndPoint1.coordinateX) * (EndPoint2.coordinateX - EndPoint1.coordinateX) +
				(p.coordinateY - EndPoint1.coordinateY) * (EndPoint2.coordinateY - EndPoint1.coordinateY);
			if (dotProduct < 0)
			{
				return false;
			}

			var squaredDistance = Math.Pow(EndPoint2.coordinateX - EndPoint1.coordinateX, 2) + 
				Math.Pow(EndPoint2.coordinateY - EndPoint1.coordinateY, 2);
			if (dotProduct > squaredDistance)
			{
				return false;
			}

			return true;
		}

		private (LineSegment, LineSegment) SplitAtPoint(Point pointToSplit)
		{
			if (!this.ContainsPoint(pointToSplit))
			{
				throw new ArgumentException("Point not on segment");
			}
			return (new LineSegment(EndPoint1, pointToSplit), new LineSegment(pointToSplit, EndPoint2));
		}

		/// <summary>
		/// Split the segment at every point in the list
		/// </summary>
		/// <param name="pointsToSplit"> A list of points on the segment at which to split; the do not have to be unique. </param>
		/// <returns></returns>
		internal List<LineSegment> SplitAtPoints(List<Point> pointsToSplit)
		{
			var splitSegments = new List<LineSegment>();

			// make sure all points are distinct and ordered in ascending or descending order by X or Y coordinate so that the elements at the front are closer to EndPoint1
			List<Point> sortedPointsToSplit;
			if (EndPoint1.coordinateX != EndPoint2.coordinateX)
			{
				sortedPointsToSplit = EndPoint1.coordinateX < EndPoint2.coordinateX ?
							pointsToSplit.OrderBy(o => o.coordinateX).ToList() :
							pointsToSplit.OrderByDescending(o => o.coordinateX).ToList();
			} 
			else
			{
				Debug.Assert(EndPoint1.coordinateY != EndPoint2.coordinateY);
				sortedPointsToSplit = EndPoint1.coordinateY < EndPoint2.coordinateY ?
										pointsToSplit.OrderBy(o => o.coordinateY).ToList() :
										pointsToSplit.OrderByDescending(o => o.coordinateY).ToList();
			}
			List<Point> uniqueSortedPointsToSplit = sortedPointsToSplit.Distinct().ToList();

			var remainingSegment = this;
			for (int i = 0; i < uniqueSortedPointsToSplit.Count; i++)
			{
				var result = remainingSegment.SplitAtPoint(uniqueSortedPointsToSplit[i]);
				splitSegments.Add(result.Item1); 
				remainingSegment = result.Item2;
			}
			splitSegments.Add(remainingSegment); // the last segment

			return splitSegments;
		}
	}
}
