using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	/// <summary>
	/// A straight line segment defined by its two end points. The two end points cannot be the same point. 
	/// FirstPoint always has smaller or equal X coordinate compared to SecondPoint; 
	/// if they are equal, FirstPoint always has smaller Y coordinate.
	/// </summary>
	/// 
	readonly struct LineSegment : IEquatable<LineSegment>
	{
		public Point FirstPoint { get; }
		public Point SecondPoint { get; }

		internal LineSegment(Point endPoint1, Point endPoint2)
		{		
			if (endPoint1 == endPoint2)
			{
				throw new ArgumentException("Parameters cannot be identical");
			}

			if (endPoint1.coordinateX < endPoint2.coordinateX)
			{
				this.FirstPoint = endPoint1;
				this.SecondPoint = endPoint2;
			}
			else if (endPoint1.coordinateX > endPoint2.coordinateX)
			{
				this.FirstPoint = endPoint2;
				this.SecondPoint = endPoint1;
			}
			else
			{
				Debug.Assert(endPoint1.coordinateX == endPoint2.coordinateX);
				if (endPoint1.coordinateY < endPoint2.coordinateY)
				{
					this.FirstPoint = endPoint1;
					this.SecondPoint = endPoint2;
				}
				else
				{
					Debug.Assert(endPoint1.coordinateY > endPoint2.coordinateY);
					this.FirstPoint = endPoint2;
					this.SecondPoint = endPoint1;
				}
			} 
		}

		/// <summary>
		/// Line segments do not intersect if they overlap or merely share an endpoint. 
		/// They do intersect if they form a T shape. 
		/// </summary>
		/// <param name="ls"> A LineSegment to check for intersection </param>
		/// <returns> the point of intersection; null if not found </returns>
		internal Point? FindIntersection(LineSegment ls)
		{
			if (this.FirstPoint == ls.FirstPoint || this.FirstPoint == ls.SecondPoint || 
				this.SecondPoint == ls.FirstPoint || this.SecondPoint == ls.SecondPoint)
			{
				// end points overlap
				return null;
			}

			var p1x = this.FirstPoint.coordinateX;
			var p1y = this.FirstPoint.coordinateY;
			var p2x = this.SecondPoint.coordinateX;
			var p2y = this.SecondPoint.coordinateY;

			var p3x = ls.FirstPoint.coordinateX;
			var p3y = ls.FirstPoint.coordinateY;
			var p4x = ls.SecondPoint.coordinateX;
			var p4y = ls.SecondPoint.coordinateY;

			var s1x = p2x - p1x;
			var s1y = p2y - p1y;
			var s2x = p4x - p3x;
			var s2y = p4y - p3y;

			if (Math.Abs(s1x / s1y - s2x / s2y) < double.Epsilon)
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
			if (p == FirstPoint || p == SecondPoint)
			{
				return false;
			}

			var crossProduct = (p.coordinateY - FirstPoint.coordinateY) * (SecondPoint.coordinateX - FirstPoint.coordinateX) -
				(p.coordinateX - FirstPoint.coordinateX) * (SecondPoint.coordinateY - FirstPoint.coordinateY);
			if (Math.Abs(crossProduct) > double.Epsilon)
			{
				// the three points are not aligned
				return false;
			}

			var dotProduct = (p.coordinateX - FirstPoint.coordinateX) * (SecondPoint.coordinateX - FirstPoint.coordinateX) +
				(p.coordinateY - FirstPoint.coordinateY) * (SecondPoint.coordinateY - FirstPoint.coordinateY);
			if (dotProduct < 0)
			{
				return false;
			}

			var squaredDistance = Math.Pow(SecondPoint.coordinateX - FirstPoint.coordinateX, 2) + 
				Math.Pow(SecondPoint.coordinateY - FirstPoint.coordinateY, 2);
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
			return (new LineSegment(FirstPoint, pointToSplit), new LineSegment(pointToSplit, SecondPoint));
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
			if (FirstPoint.coordinateX != SecondPoint.coordinateX)
			{
				sortedPointsToSplit = FirstPoint.coordinateX < SecondPoint.coordinateX ?
							pointsToSplit.OrderBy(o => o.coordinateX).ToList() :
							pointsToSplit.OrderByDescending(o => o.coordinateX).ToList();
			} 
			else
			{
				Debug.Assert(FirstPoint.coordinateY != SecondPoint.coordinateY);
				sortedPointsToSplit = FirstPoint.coordinateY < SecondPoint.coordinateY ?
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

		private static bool AreParallel(LineSegment ls1, LineSegment ls2)
		{
			var p1x = ls1.FirstPoint.coordinateX;
			var p1y = ls1.FirstPoint.coordinateY;
			var p2x = ls1.SecondPoint.coordinateX;
			var p2y = ls1.SecondPoint.coordinateY;

			var p3x = ls2.FirstPoint.coordinateX;
			var p3y = ls2.FirstPoint.coordinateY;
			var p4x = ls2.SecondPoint.coordinateX;
			var p4y = ls2.SecondPoint.coordinateY;

			var s1x = p2x - p1x;
			var s1y = p2y - p1y;
			var s2x = p4x - p3x;
			var s2y = p4y - p3y;

			return (Math.Abs(s1x / s1y - s2x / s2y) < double.Epsilon); // return true iff the slops are equal
		}


		/// <summary>
		/// Check whether the two segments overlap. 
		/// If they do, split them into shortest non-overlapping segments and return those;
		/// If they don't, return the two segments as they are
		/// </summary>
		/// <param name="ls"></param>
		/// <returns></returns>
		internal static List<LineSegment> SplitIfOverlap(LineSegment ls1, LineSegment ls2)
		{
			var splitSegments = new List<LineSegment>();
			
			if (ls1.FirstPoint == ls2.FirstPoint && ls1.SecondPoint == ls2.SecondPoint)
			{
				// complete overlap
				splitSegments.Add(ls1);
				return splitSegments;
			}
			if (LineSegment.AreParallel(ls1, ls2))
			{
				if (ls1.ContainsPoint(ls2.FirstPoint) || 
					ls1.ContainsPoint(ls2.SecondPoint) ||
					ls2.ContainsPoint(ls1.FirstPoint) ||
					ls2.ContainsPoint(ls1.SecondPoint))
				{
					// they partially overlap. Put the unique endpoints in order
					var endPoints = new List<Point> { ls1.FirstPoint, ls1.SecondPoint, ls2.FirstPoint, ls2.SecondPoint };
					List<Point> sortedEndPointsByY = endPoints.OrderBy(o => o.coordinateY).ToList();
					List<Point> sortedEndPoints = sortedEndPointsByY.OrderBy(o => o.coordinateX).ToList();
					List<Point> uniqueSortedPoints = sortedEndPoints.Distinct().ToList();

					// make the segments connecting the endpoints
					for (int i = 0; i< uniqueSortedPoints.Count - 1; i++)
					{
						splitSegments.Add(new LineSegment(uniqueSortedPoints[i], uniqueSortedPoints[i + 1]));
					}

					return splitSegments;
				}
			}
			splitSegments.Add(ls1);
			splitSegments.Add(ls2);
			return splitSegments;
		}

		public override int GetHashCode() => (this.FirstPoint, this.SecondPoint).GetHashCode();

		public static bool operator == (LineSegment lhs, LineSegment rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator != (LineSegment lhs, LineSegment rhs) => !(lhs == rhs);

		public override bool Equals(object obj) => obj is LineSegment other && this.Equals(other);
		bool IEquatable<LineSegment>.Equals(LineSegment other) => this.Equals(other);

		private bool Equals(LineSegment ls)
		{
			return (this.FirstPoint == ls.FirstPoint && this.SecondPoint == ls.SecondPoint);
		}
	}
}
