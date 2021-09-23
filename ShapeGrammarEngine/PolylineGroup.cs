using BasicGeometries;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine
{
	public class PolylineGroup
	{
		public readonly List<List<(double X, double Y)>> Polylines = new List<List<(double, double)>>();

		public PolylineGroup(List<List<(double, double)>> polylines)
		{
			if (polylines is null)
			{
				throw new ArgumentNullException();
			}
			this.Polylines = polylines;
			this.CleanUp();
		}

		public static PolylineGroup CreateEmptyPolylineGroup() {
			return new PolylineGroup(new List<List<(double, double)>>());
		}

		/// <summary>
		/// Remove any polyline with less than 2 points and thereby doesn't form a line
		/// </summary>
		private void CleanUp()
		{
			var indexesToRemove = new List<int>();
			for (int i = 0; i < this.Polylines.Count; i++)
			{
				var pl = this.Polylines[i];
				if (pl.Count < 2)
				{
					indexesToRemove.Add(i);
				}
			}
			indexesToRemove.Sort();
			indexesToRemove.Reverse();
			foreach (int index in indexesToRemove)
			{
				this.Polylines.RemoveAt(index);
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
			foreach (List<(double X, double Y)> polyline in this.Polylines)
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
			foreach (List<(double, double)> polyline in this.Polylines)
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
