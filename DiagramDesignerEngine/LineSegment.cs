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
	public readonly struct LineSegment : IEquatable<LineSegment>
	{
		public Point FirstPoint { get; }
		public Point SecondPoint { get; }

		public LineSegment(Point endPoint1, Point endPoint2)
		{		
			if (endPoint1 == endPoint2)
			{
				throw new ArgumentException("endpoints cannot be identical");
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
			
			if (s1y != 0 && s2y != 0)
			{
				if (Math.Abs(s1x / s1y - s2x / s2y) < double.Epsilon)
				{
					// slops are the same: they are parallel or overlap
					return null;
				}
			}
			else
			{
				if (Math.Abs(s1y / s1x - s2y / s2x) < double.Epsilon)
				{
					// slops are the same: they are parallel or overlap
					return null;
				}
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
		/// Check whether the point is on the segment
		/// </summary>
		/// <param name="p"> The point to check </param>
		/// <returns> false if the point is not on the segment </returns>
		internal bool ContainsPoint(Point p)
		{
			double maximumDistanceTolerance = 0.00001;
			var A = p.coordinateX - this.FirstPoint.coordinateX;
			var B = p.coordinateY - this.FirstPoint.coordinateY;
			var C = this.SecondPoint.coordinateX - this.FirstPoint.coordinateX;
			var D = this.SecondPoint.coordinateY - this.FirstPoint.coordinateY;

			var dot = A * C + B * D;
			var len_sq = C * C + D * D;
			double param = -1;
			Debug.Assert(len_sq != 0);
			param = dot / len_sq;

			if (param >= 0 && param <= 1)
			{
				var projX = this.FirstPoint.coordinateX + param * C;
				var projY = this.FirstPoint.coordinateY + param * D;
				var projPoint = new Point(projX, projY);
				var distance = Point.DistanceBetweenPoints(p, projPoint);

				return distance <= maximumDistanceTolerance;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Split the segment at the point
		/// </summary>
		/// <param name="pointToSplit"></param>
		/// <returns> The split segments; 
		/// if the point is at FirstPoint, the first item is null, and the second item is the original segment
		/// if the point is at SecondPoint, the first item is the original segment, and the second item is null </returns>
		private (LineSegment?, LineSegment?) SplitAtPoint(Point pointToSplit)
		{
			if (!this.ContainsPoint(pointToSplit))
			{
				throw new ArgumentException("Point not on segment");
			}

			if (pointToSplit == this.FirstPoint)
			{
				return (null, new LineSegment(pointToSplit, SecondPoint));
			}
			else if (pointToSplit == this.SecondPoint)
			{
				return (new LineSegment(pointToSplit, FirstPoint), null);
			}

			return (new LineSegment(FirstPoint, pointToSplit), new LineSegment(pointToSplit, SecondPoint));
		}

		/// <summary>
		/// Split the segment at every point in the list
		/// </summary>
		/// <param name="pointsToSplit"> A list of points on the segment at which to split; the do not have to be unique. Can be empty </param>
		/// <returns> The list of split segments, or the original segment if list is empty </returns>
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

			LineSegment? remainingSegment = this;
			for (int i = 0; i < uniqueSortedPointsToSplit.Count; i++)
			{
				var result = ((LineSegment)remainingSegment).SplitAtPoint(uniqueSortedPointsToSplit[i]);
				if (!(result.Item1 is null))
				{
					splitSegments.Add((LineSegment)result.Item1);
				}

				remainingSegment = result.Item2;
				if (remainingSegment is null) { break; }
			}

			if (!(remainingSegment is null))
			{
				splitSegments.Add((LineSegment)remainingSegment); // the last segment
			}
			
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

			// return true iff the slops are equal
			if (s1y != 0 && s2y != 0)
			{
				if (Math.Abs(s1x / s1y - s2x / s2y) < double.Epsilon)
				{
					// slops are the same: they are parallel or overlap
					return true;
				}
			}
			else
			{
				if (Math.Abs(s1y / s1x - s2y / s2x) < double.Epsilon)
				{
					// slops are the same: they are parallel or overlap
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Whether two segments overlap, either partially or fully
		/// </summary>
		/// <param name="ls1"> one segment </param>
		/// <param name="ls2"> another segment </param>
		/// <returns> whether they overlap, either partially or fully </returns>
		internal static bool DoOverlap(LineSegment ls1, LineSegment ls2)
		{
			if (LineSegment.AreParallel(ls1, ls2))
			{
				int numOfContainment = 0;
				if (ls1.ContainsPoint(ls2.FirstPoint)) { numOfContainment++; }
				if (ls1.ContainsPoint(ls2.SecondPoint)) { numOfContainment++; }
				if (ls2.ContainsPoint(ls1.FirstPoint)) { numOfContainment++; }
				if (ls2.ContainsPoint(ls1.SecondPoint)) { numOfContainment++; }
				
				if (numOfContainment < 2)
				{
					// they are not touching
					return false;
				}
				else if (numOfContainment == 2)
				{
					if (ls1.FirstPoint == ls2.FirstPoint ||
						ls1.FirstPoint == ls2.SecondPoint ||
						ls1.SecondPoint == ls2.FirstPoint ||
						ls1.SecondPoint == ls2.SecondPoint)
					{
						// they are merely touching at endpoint
						return false;
					}
					else
					{
						return true;
					}
				}
				else
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// If the two segments overlap, return the points at which each of them should split
		/// such that the resulting segments either completely overlap (identical) or do not overlap at all
		/// </summary>
		/// <param name="ls1"></param>
		/// <param name="ls2"></param>
		/// <returns> the list of points to split; 
		/// return an empty list if the segments completely overlap or do not overlap at all </returns>
		internal static List<Point> PointsToSplitIfOverlap(LineSegment ls1, LineSegment ls2)
		{
			if (LineSegment.DoOverlap(ls1, ls2))
			{
				// they overlap. Put the unique endpoints in order. (doesn't matter it's ascending or descending as long as it's sorted)
				var endPoints = new List<Point> { ls1.FirstPoint, ls1.SecondPoint, ls2.FirstPoint, ls2.SecondPoint };
				List<Point> sortedEndPointsByY = endPoints.OrderBy(o => o.coordinateY).ToList();
				List<Point> sortedEndPoints = sortedEndPointsByY.OrderBy(o => o.coordinateX).ToList();
				List<Point> uniqueSortedPoints = sortedEndPoints.Distinct().ToList();

				// take only the points in the middle (if any)
				var pointsToSplit = uniqueSortedPoints;
				pointsToSplit.RemoveAt(uniqueSortedPoints.Count - 1);
				pointsToSplit.RemoveAt(0);
				return pointsToSplit;
			}
			// they do not overlap
			return new List<Point>(); 
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

		public override string ToString()
		{
			return String.Concat(this.FirstPoint.ToString(), " ", this.SecondPoint.ToString());
		}
	}
}
