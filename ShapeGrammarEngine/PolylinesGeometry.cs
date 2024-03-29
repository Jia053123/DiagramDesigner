﻿using BasicGeometries;
using ListOperations;
using System;
using System.Collections.Generic;
using DiagramDesignerGeometryParser;
using System.Linq;

namespace ShapeGrammarEngine
{
	public class PolylinesGeometry
	{
		private List<List<Point>> polylines = new List<List<Point>>();
		/// <summary>
		/// The polylines forming the geometry; They must not intersect or overlap with itself or each other. 
		/// (while removing this requriement does not break anything, it violates the basic premise of the definition of shape in this project) 
		/// Each polyline consists of at least 2 points
		/// </summary>
		public List<List<Point>> PolylinesCopy { get { return new List<List<Point>>(this.polylines); } }

		public PolylinesGeometry(List<List<Point>> polylines)
		{
			if (polylines is null)
			{
				throw new ArgumentNullException();
			}
			this.polylines = polylines;

			this.CleanUpPolylines();
			if (this.DoesIntersectOrOverlapWithItself())
			{
				// TODO: remove intersections and overlaps without flattening the polyline structure
				var segments = this.ConvertToLineSegments();
				var exploder = new LineSegmentsExploder(segments);
				var explodedSegments = exploder.MergeAndExplodeSegments();
				var explodedLines = new List<List<Point>>();
				foreach (LineSegment ls in explodedSegments)
				{
					explodedLines.Add(new List<Point>{ ls.FirstPoint, ls.SecondPoint });
				}
				this.polylines = explodedLines;
			}
		}

		public static PolylinesGeometry CreateEmptyPolylineGeometry() {
			return new PolylinesGeometry(new List<List<Point>>());
		}

		public bool IsEmpty()
		{
			return this.polylines.Count == 0;
		}

		public HashSet<Point> GetAllPoints()
		{
			var allPoints = new HashSet<Point>();
			foreach (List<Point> polyline in this.polylines)
			{
				foreach (Point point in polyline)
				{
					allPoints.Add(point);
				}
			}
			return allPoints;
		}

		/// <summary>
		/// Try to merge fragmented line segments into long, continuous polylines. 
		/// After this is called, no two polylines in this geometry will share a common endpoint. 
		/// Note that the result may not be the only valid solution
		/// </summary>
		public void MergePolylines()
		{
			// TODO: implement this to potentially speed up shape matching
			throw new NotImplementedException();
		}

		/// <summary>
		/// Merge two polylines if possible
		/// </summary>
		static private List<Point> MergeTwoPolylines(List<Point> polyline1, List<Point> polyline2)
		{
			List<Point> mergedPolyline = new List<Point>();
			if (polyline1.Last() == polyline2.First())
			{
				mergedPolyline.AddRange(polyline1);
				mergedPolyline.AddRange(polyline2);
				return mergedPolyline;
			}
			else if (polyline2.Last() == polyline1.First())
			{
				mergedPolyline.AddRange(polyline2);
				mergedPolyline.AddRange(polyline1);
				return mergedPolyline;
			}
			else if (polyline1.First() == polyline2.First())
			{
				mergedPolyline.AddRange(polyline1);
				mergedPolyline.Reverse();
				mergedPolyline.AddRange(polyline2);
				return mergedPolyline;
			}
			else if (polyline1.Last() == polyline2.Last())
			{
				mergedPolyline.AddRange(polyline1);
				var temp = new List<Point>(polyline2);
				temp.Reverse();
				mergedPolyline.AddRange(temp);
				return mergedPolyline;
			}
			else
			{
				throw new ArgumentException("the two polylines cannot be merged");
			}
		}
		
		/// <summary>
		/// Remove any polyline with less than 2 points and thereby doesn't form a line
		/// </summary>
		private void CleanUpPolylines()
		{
			var indexesToRemove = new List<int>();
			for (int i = 0; i < this.polylines.Count; i++)
			{
				var pl = this.polylines[i];
				if (pl.Count < 2)
				{
					indexesToRemove.Add(i);
				}
			}
			indexesToRemove.Sort();
			indexesToRemove.Reverse();
			foreach (int index in indexesToRemove)
			{
				this.polylines.RemoveAt(index);
			}
		}

		private bool DoesIntersectOrOverlapWithItself()
		{
			var allSegments = this.ConvertToLineSegments();
			for (int i = 0; i < allSegments.Count; i++)
			{
				for (int j = i + 1; j < allSegments.Count; j++)
				{
					if (!(allSegments[i].FindIntersection(allSegments[j]) is null))
					{
						return true;
					}
					if (LineSegment.DoOverlap(allSegments[i], allSegments[i + 1]))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Add a single segment to the geometry, potentially merging two polylines
		/// </summary>
		/// <param name="endPoint1"> One of the two endpoints </param>
		/// <param name="endPoint2"> One of the two endpoints </param>
		public void AddSegmentByPoints(Point endPoint1, Point endPoint2)
		{
			if (endPoint1 == endPoint2)
			{
				throw new ArgumentException("two endpoints cannot be identical");
			}

			var newPolyline = new List<Point> { endPoint1, endPoint2 };
			this.polylines.Add(newPolyline);

			this.CleanUpPolylines();
		}

		/// <summary>
		/// Erase a single segment from the geometry, potentially breaking up a polyline. 
		/// Because the geometry is not supposed to overlap with itself, there can be at most one eligiable segment. 
		/// </summary>
		/// <param name="endPoint1"> One of the two endpoints </param>
		/// <param name="endPoint2"> One of the two endpoints </param>
		/// <returns> whether the operation succeeded </returns>
		public bool EraseSegmentByPoints(Point endPoint1, Point endPoint2)
		{
			if (endPoint1 == endPoint2)
			{
				throw new ArgumentException("two endpoints cannot be identical");
			}
			for (int i = 0; i < this.polylines.Count; i++)
			{
				var pl = this.polylines[i];
				for (int j = 0; j < pl.Count - 1; j++)
				{
					if ((pl[j] == endPoint1 && pl[j+1] == endPoint2) || 
						(pl[j] == endPoint2 && pl[j+1] == endPoint1))
					{
						this.EraseSegmentByIndexes(i, j);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Erase a single line segment from the geometry, potentially breaking up a polyline. 
		/// </summary>
		/// <param name="polylineIndex"> the index of the polyline that contains the segment </param>
		/// <param name="startPointIndex"> the index first endpoint of the segment; 
		/// the index for the second endpoint is this index plus 1 </param>
		public void EraseSegmentByIndexes(int polylineIndex, int startPointIndex)
		{
			if (polylineIndex < 0 || polylineIndex > this.polylines.Count - 1)
			{
				throw new ArgumentOutOfRangeException("polylineIndex is out of range");
			}
			if (startPointIndex < 0 || startPointIndex > this.polylines[polylineIndex].Count - 2)
			{
				throw new ArgumentOutOfRangeException("startPointIndex is out of range");
			}

			List<Point> polylineBefore = null;
			List<Point> polylineAfter = null;

			var polyline = this.polylines[polylineIndex];
			polylineBefore = polyline.GetRange(0, startPointIndex + 1);
			polylineAfter = polyline.GetRange(startPointIndex + 1, polyline.Count - 1 - startPointIndex);

			this.polylines.RemoveAt(polylineIndex);
			if (polylineAfter is object && polylineAfter.Count > 0)
			{
				this.polylines.Insert(polylineIndex, polylineAfter);
			}
			if (polylineBefore is object && polylineBefore.Count > 0)
			{
				this.polylines.Insert(polylineIndex, polylineBefore);
			}

			this.CleanUpPolylines();
		}

		private List<LineSegment> ConvertToLineSegments()
		{
			var allSegments = new List<LineSegment>();
			foreach (List<Point> polyline in this.polylines)
			{
				for (int i = 0; i < polyline.Count - 1; i++)
				{
					var p = new Point(polyline[i].coordinateX, polyline[i].coordinateY);
					var nextP = new Point(polyline[i + 1].coordinateX, polyline[i + 1].coordinateY);
					allSegments.Add(new LineSegment(p, nextP));
				}
			}
			return allSegments;
		}

		public HashSet<Connection> ConvertToConnections(LabelingDictionary labeling)
		{
			var connections = new HashSet<Connection>();
			foreach (List<Point> polyline in this.polylines)
			{
				for (int i = 0; i < polyline.Count - 1; i++)
				{
					var p1 = polyline[i];
					var p2 = polyline[i + 1];

					int label1, label2;
					try
					{
						label1 = labeling.GetLabelByPoint(p1);
						label2 = labeling.GetLabelByPoint(p2);
					}
					catch (ArgumentException)
					{
						throw new ArgumentException("labeling dictionary doesn't match with the polylines");
					}
					
					var c = new Connection(label1, label2);
					connections.Add(c);
				}
			}
			return connections;
		}

		public bool IsGeometryIdenticalWith(PolylinesGeometry pg)
		{
			return false;
		}

		/// <summary>
		/// get a point in the polylines geometry by index
		/// </summary>
		/// <param name="pointIndex"> index of the point on the polyline </param>
		/// <param name="polylineIndex"> index of the polyline in the geometry </param>
		/// <returns> the point specified by index  </returns>
		/// <exception cref="ArgumentOutOfRangeException"> throws when an index is out of range </exception>
		public Point GetPointByIndex(int pointIndex, int polylineIndex)
		{
			return this.polylines[polylineIndex][pointIndex];
		}

		/// <summary>
		/// Find the next point index and polyline index for the next point in the polylines; 
		/// If the input is 0, -1, output (0, 0)
		/// </summary>
		/// <param name="currentPointIndex"> the index of the point on its polyline </param>
		/// <param name="currentPolylineIndex"> the index of the polyline where the point belongs to in this geometry </param>
		/// <returns> the two indexes for the next point on the geometry; if at the end of the geometry, output (-1, -1) </returns>
		/// <exception cref="ArgumentException"> throws when the input is not an existing point in this geometry and not 0, -1 </exception>
		public (int nextPointIndex, int nextPolylineIndex) FindIndexForNextPoint(int currentPointIndex, int currentPolylineIndex)
		{
			if (currentPointIndex == 0 && currentPolylineIndex == -1)
			{
				return (0, 0);
			}
			if (currentPointIndex != 0 && currentPolylineIndex == -1)
			{
				throw new ArgumentException("polyline index is -1 but point index not 0");
			}
			if (currentPolylineIndex < -1 || this.polylines.Count - 1 < currentPolylineIndex)
			{
				throw new ArgumentException("polyline index out of range");
			}
			if (currentPointIndex < 0 || this.polylines[currentPolylineIndex].Count - 1 < currentPointIndex)
			{
				throw new ArgumentException("point index out of range");
			}

			var currentPolyline = this.polylines[currentPolylineIndex];
			if (currentPolyline.Count - 1 > currentPointIndex)
			{
				// haven't reached the end of the current line
				return (currentPointIndex + 1, currentPolylineIndex);
			}
			else if (this.polylines.Count - 1 > currentPolylineIndex)
			{
				// end of the current polyline, but there are more lines
				return (0, currentPolylineIndex + 1);
			}
			else
			{
				// end of the geometry
				return (-1, -1);
			}
		}
	}
}
