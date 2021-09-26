using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
		/// The output shape is guaranteed to conform with the input geometry. 
		/// If labeling is null, label the shape with consecutive integers from 0; otherwise, label according to the labeling. 
		/// If a point not in the dictionary is found, new labels are generated as consecutive integers starting from the largest label in the dictionary
		/// </summary>
		/// <param name="polylines"> the input geometry.</param>
		/// <param name="labeling"> the pre-defined labels for specific points </param>
		/// <returns> The output shape. </returns>
		public static Shape CreateShapeFromPolylines(PolylineGroup polylineGroup, Dictionary<Point, int> labeling)
		{
			if (polylineGroup is null)
			{
				throw new ArgumentNullException();
			}

			if (polylineGroup.PolylinesCopy.Count == 0)
			{
				return Shape.CreateEmptyShape();
			}

			if (polylineGroup.DoesIntersectOrOverlapWithItself())
			{
				throw new ArgumentException("polylineGroup intersects or overlaps with itself");
			}

			// label all unique points
			Dictionary<Point, int> labelDictionary;
			int nextNewLabel;
			if (labeling is null)
			{
				labelDictionary = new Dictionary<Point, int>();
				nextNewLabel = 0;
			}
			else
			{
				labelDictionary = labeling;
				List<int> allLabels = labelDictionary.Values.ToList();
				allLabels.Sort();
				nextNewLabel = allLabels.Last() + 1;
			}
			
			foreach (List<Point> polyline in polylineGroup.PolylinesCopy)
			{
				foreach (Point p in polyline)
				{
					if (!labelDictionary.ContainsKey(p))
					{
						labelDictionary.Add(p, nextNewLabel);
						nextNewLabel++;
					}
				}
			}

			var connections = polylineGroup.ConvertToConnections(labelDictionary);
		
			var newShape = new Shape(connections);
			Debug.Assert(newShape.ConformsWithGeometry(polylineGroup, out _));
			return newShape;
		}

		/// <summary>
		/// Wheher the input is of this shape
		/// </summary>
		/// <param name="polylineGroup"> the polyline geometry to check against this shape </param>
		/// <param name="labeling"> if the input is of this shape, output how each point in the polylines is labeled 
		/// (not guaranteed to be the only solution); otherwise output null </param>
		/// <returns> whether the intput is of this shape </returns>
		public bool ConformsWithGeometry(PolylineGroup polylineGroup, out Dictionary<Point, int> labeling)
		{
			if (polylineGroup is null)
			{
				throw new ArgumentNullException();
			}

			if (polylineGroup.PolylinesCopy.Count == 0)
			{
				if (this.Definition.Count == 0)
				{
					labeling = new Dictionary<Point, int>();
					return true;
				}
				else
				{
					labeling = null;
					return false;
				}
			}

			// step1: find all unique points in the polylines and check if the count is the same as the count of labels in shape
			var uniqueCoordinates = new HashSet<Point>();
			foreach (List<Point> pl in polylineGroup.PolylinesCopy)
			{
				uniqueCoordinates.UnionWith(pl);
			}
			var uniqueCoordinatesList = new List<Point>(uniqueCoordinates);
			if (uniqueCoordinates.Count != this.GetAllLabels().Count)
			{
				labeling = null;
				return false;
			}

			// step2: generate all potential ways each unique point can be labeled
			var allPotentialLabeling = Utilities.GenerateAllPermutations(new List<int>(this.GetAllLabels()));

			// step3: check if there is one potential labeling with which the input would match the definition of this shape
			foreach (List<int> l in allPotentialLabeling)
			{
				Debug.Assert(uniqueCoordinatesList.Count == l.Count);
				var labelDictionary = new Dictionary<Point, int>();
				for (int i = 0; i < uniqueCoordinatesList.Count; i++)
				{
					labelDictionary.Add(uniqueCoordinatesList[i], l[i]);
				}

				var connections = polylineGroup.ConvertToConnections(labelDictionary);

				if (this.Definition.SetEquals(connections))
				{
					labeling = labelDictionary;
					return true;
				}
			}
			labeling = null;
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
}
