using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ShapeGrammarEngine
{
	/// <summary>
	/// A shape in the context of shape grammar. Its definition is independent to any transformation that can 
	/// potentially be applied to it. 
	/// In this project, a shape is defined as a graph that specifies the connections among the points
	/// </summary>
	public readonly struct Shape: IEquatable<Shape>
	{
		/// <summary>
		/// A shape is defined as a graph: each tuple in the set represents a connection in the graph between two nodes. 
		/// Each node is represented by a unique label in the form of an integer. All labels in a definition 
		/// </summary>
		public readonly HashSet<Connection> Definition;

		public Shape(HashSet<Connection> definition)
		{
			this.Definition = definition;
		}

		/// <summary>
		/// Extract a graph-based shape instance from a geometry. The geometry must not intersect with itself
		/// The output shape is guaranteed to conform with the input geometry
		/// </summary>
		/// <param name="polylines"></param>
		/// <returns></returns>
		public static Shape CreateShapeFromPolylines(List<List<(double, double)>> polylines)
		{
			if (polylines is null)
			{
				throw new ArgumentNullException();
			}

			// Check for intersections and overlaps
			var allSegments = new List<LineSegment>();
			foreach (List<(double X, double Y)> polyline in polylines)
			{
				for (int i = 0; i < polyline.Count - 1; i++)
				{
					var p = new Point(polyline[i].X, polyline[i].Y);
					var nextP = new Point(polyline[i+1].X, polyline[i+1].Y);
					allSegments.Add(new LineSegment(p, nextP));
				}
			}

			for (int i = 0; i < allSegments.Count; i++)
			{
				for (int j = i+1; j < allSegments.Count; j++)
				{
					if (!(allSegments[i].FindIntersection(allSegments[j]) is null))
					{
						throw new ArgumentException("the input intersects with itself");
					}
					if (LineSegment.DoOverlap(allSegments[i], allSegments[i+1]))
					{
						throw new ArgumentException("the input overlaps with itself");
					}
				}
			}

			// step1: label all unique points
			var labelDictionary = new Dictionary<(double X, double Y), int>();
			int label = 0;
			foreach (List<(double, double)> polyline in polylines)
			{
				foreach ((double X, double Y) p in polyline)
				{
					if (!labelDictionary.ContainsKey(p))
					{
						labelDictionary.Add(p, label);
						label++;
					}
				}
			}

			// step2: convert all line segments to connections
			var connections = new HashSet<Connection>();
			foreach (List<(double, double)> polyline in polylines)
			{
				for (int i = 0; i < polyline.Count - 1; i++)
				{
					var p1 = polyline[i];
					var p2 = polyline[i + 1];
					int label1, label2;
					var s1 = labelDictionary.TryGetValue(p1, out label1);
					var s2 = labelDictionary.TryGetValue(p2, out label2);
					Debug.Assert(s1 && s2);

					var c = new Connection(label1, label2);
					connections.Add(c);
				}
			}

			// step3: make new shape
			var newShape = new Shape(connections);
			return newShape;

			// TODO: Debug.Assert conformity 
		}

		public bool ConformsWithGeometry(List<List<(double X, double Y)>> polylines)
		{
			if (polylines is null)
			{
				throw new ArgumentNullException();
			}
			return true;
			// step1: 
		}

		public static bool operator ==(Shape lhs, Shape rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Shape lhs, Shape rhs) => !(lhs == rhs);

		public override bool Equals(object obj) => obj is Shape other && this.Equals(other);
		bool IEquatable<Shape>.Equals(Shape other) => this.Equals(other);

		private bool Equals(Shape s)
		{
			if (this.Definition.Count != s.Definition.Count)
			{
				return false;
			}

			foreach (Connection c in this.Definition)
			{
				if (! s.Definition.Contains(c))
				{
					return false;
				}
			}

			return true;
		}
	}

	class FailedToBuildShapeException: Exception { }
}
