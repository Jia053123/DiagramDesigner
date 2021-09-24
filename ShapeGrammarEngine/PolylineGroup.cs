using BasicGeometries;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine
{
	public class PolylineGroup
	{
		private List<List<(double X, double Y)>> polylines = new List<List<(double, double)>>();
		/// <summary>
		/// each polyline has at least 2 points
		/// </summary>
		public List<List<(double X, double Y)>> PolylinesCopy { get { return new List<List<(double X, double Y)>>(this.polylines); } }

		public PolylineGroup(List<List<(double, double)>> polylines)
		{
			if (polylines is null)
			{
				throw new ArgumentNullException();
			}
			this.polylines = polylines;
			this.CleanUpPolylines();
		}

		public static PolylineGroup CreateEmptyPolylineGroup() {
			return new PolylineGroup(new List<List<(double, double)>>());
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

		private List<LineSegment> ConvertToLineSegments()
		{
			var allSegments = new List<LineSegment>();
			foreach (List<(double X, double Y)> polyline in this.polylines)
			{
				for (int i = 0; i < polyline.Count - 1; i++)
				{
					var p = new Point(polyline[i].X, polyline[i].Y);
					var nextP = new Point(polyline[i + 1].X, polyline[i + 1].Y);
					allSegments.Add(new LineSegment(p, nextP));
				}
			}
			return allSegments;
		}

		public HashSet<Connection> ConvertToConnections(Dictionary<(double X, double Y), int> labeling)
		{
			var connections = new HashSet<Connection>();
			foreach (List<(double, double)> polyline in this.polylines)
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
