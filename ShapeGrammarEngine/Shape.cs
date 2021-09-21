using System;
using System.Collections.Generic;
using System.Diagnostics;

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
		/// Each node is represented by a unique label in the form of a positive integer. 
		/// </summary>
		public readonly HashSet<Connection> Definition;

		public Shape(HashSet<Connection> definition)
		{
			this.Definition = definition;
		}

		public HashSet<int> GetAllLabels()
		{
			var output = new HashSet<int>();
			foreach (Connection c in this.Definition)
			{
				output.Add(c.LabelOfFirstNode);
				output.Add(c.LabelOfSecondNode);
			}
			return output;
		}

		public static Shape CreateEmptyShape()
		{
			return new Shape(new HashSet<Connection>());
		}

		/// <summary>
		/// Extract a graph-based shape instance from a geometry. The geometry must not intersect with itself
		/// The output shape is guaranteed to conform with the input geometry
		/// </summary>
		/// <param name="polylines"> the input geometry.</param>
		/// <returns> The output shape with labels that together from a consecutive integer sequence from 0 </returns>
		public static Shape CreateShapeFromPolylines(PolylineGroup polylineGroup)
		{
			if (polylineGroup is null)
			{
				throw new ArgumentNullException();
			}

			if (polylineGroup.Polylines.Count == 0)
			{
				return Shape.CreateEmptyShape();
			}

			if (polylineGroup.DoesIntersectOrOverlapWithItself())
			{
				throw new ArgumentException("polylineGroup intersects or overlaps with itself");
			}
			
			// label all unique points
			var labelDictionary = new Dictionary<(double X, double Y), int>();
			int label = 0;
			foreach (List<(double, double)> polyline in polylineGroup.Polylines)
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

			var connections = polylineGroup.ConvertToConnections(labelDictionary);
		
			var newShape = new Shape(connections);
			Debug.Assert(newShape.ConformsWithGeometry(polylineGroup));
			return newShape;
		}

		public bool ConformsWithGeometry(PolylineGroup polylineGroup)
		{
			if (polylineGroup is null)
			{
				throw new ArgumentNullException();
			}

			if (polylineGroup.Polylines.Count == 0)
			{
				return this.Definition.Count == 0 ? true : false;
			}

			// step1: find all unique points in the polylines and check if the count is the same as the count of labels in shape
			var uniqueCoordinates = new HashSet<(double X, double Y)>();
			foreach (List<(double, double)> pl in polylineGroup.Polylines)
			{
				uniqueCoordinates.UnionWith(pl);
			}
			var uniqueCoordinatesList = new List<(double, double)>(uniqueCoordinates);
			if (uniqueCoordinates.Count != this.GetAllLabels().Count)
			{
				return false;
			}

			// step2: generate all potential ways each unique point can be labeled
			var allPotentialLabeling = Utilities.GenerateAllPermutations(new List<int>(this.GetAllLabels()));

			// step3: check if there is one potential labeling with which the input would match the definition of this shape
			foreach (List<int> labeling in allPotentialLabeling)
			{
				Debug.Assert(uniqueCoordinatesList.Count == labeling.Count);
				var labelDictionary = new Dictionary<(double X, double Y), int>();
				for (int i = 0; i < uniqueCoordinatesList.Count; i++)
				{
					labelDictionary.Add(uniqueCoordinatesList[i], labeling[i]);
				}

				var connections = polylineGroup.ConvertToConnections(labelDictionary);

				if (this.Definition.SetEquals(connections))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Whether two shapes are equivalent despite potentially using a different set of labels
		/// </summary>
		public bool AreEquivalent(Shape shape1, Shape shape2)
		{
			return false; // TODO: stub
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
