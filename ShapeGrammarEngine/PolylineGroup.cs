using BasicGeometries;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine
{
	public class PolylineGroup
	{
		private List<List<Point>> polylines = new List<List<Point>>();
		/// <summary>
		/// each polyline has at least 2 points
		/// </summary>
		public List<List<Point>> PolylinesCopy { get { return new List<List<Point>>(this.polylines); } }

		public PolylineGroup(List<List<Point>> polylines)
		{
			if (polylines is null)
			{
				throw new ArgumentNullException();
			}
			this.polylines = polylines;
			this.CleanUpPolylines();
		}

		public static PolylineGroup CreateEmptyPolylineGroup() {
			return new PolylineGroup(new List<List<Point>>());
		}

		public bool IsEmpty()
		{
			return this.polylines.Count == 0;
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

		public bool DoesIntersectOrOverlapWithItself()
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
		/// Erase a point from the group of polilines. 
		/// As a result, any line with the point as an endpoint is removed, potentially breaking a polyline into two. 
		/// If there are multiple occurances of the same point, only the instance specified is erased.
		/// </summary>
		/// <param name="polylineIndex"> the index of the polyline that contains the point </param>
		/// <param name="pointIndex"> the index of the point in the specified polyline </param>
		public void ErasePoint(int polylineIndex, int pointIndex)
		{
			
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

		public HashSet<Connection> ConvertToConnections(Dictionary<Point, int> labeling)
		{
			var connections = new HashSet<Connection>();
			foreach (List<Point> polyline in this.polylines)
			{
				for (int i = 0; i < polyline.Count - 1; i++)
				{
					var p1 = polyline[i];
					var p2 = polyline[i + 1];
					int label1, label2;
					var s1 = labeling.TryGetValue(p1, out label1);
					var s2 = labeling.TryGetValue(p2, out label2);
					if (!s1 || !s2)
					{
						throw new ArgumentException("labeling dictionary doesn't match with the polylines");
					}

					var c = new Connection(label1, label2);
					connections.Add(c);
				}
			}
			return connections;
		}
	}
}
