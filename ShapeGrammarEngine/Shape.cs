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
		public readonly HashSet<Connection> DefiningConnections;

		public Shape(HashSet<Connection> definition)
		{
			this.DefiningConnections = definition;
		}

		public HashSet<int> GetAllLabels()
		{
			var output = new HashSet<int>();
			foreach (Connection c in this.DefiningConnections)
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
		/// <param name="preDefinedLabeling"> the pre-defined labels for specific points; if null, assume it is empty </param>
		/// <returns> The output shape. </returns>
		public static Shape CreateShapeFromPolylines(PolylinesGeometry polylineGeometry, LabelingDictionary preDefinedLabeling, out LabelingDictionary newShapeLabeling)
		{
			if (polylineGeometry is null)
			{
				throw new ArgumentNullException();
			}

			if (polylineGeometry.PolylinesCopy.Count == 0)
			{
				newShapeLabeling = new LabelingDictionary();
				return Shape.CreateEmptyShape();
			}

			// label all unique points
			LabelingDictionary labelDictionary;
			int nextNewLabel;
			if (preDefinedLabeling is null)
			{
				labelDictionary = new LabelingDictionary();
				nextNewLabel = 0;
			}
			else
			{
				labelDictionary = preDefinedLabeling.Copy(); // make a copy to avoid outputing the input object
				List<int> allLabels = labelDictionary.GetAllLabels().ToList();
				allLabels.Sort();
				nextNewLabel = allLabels.Last() + 1;
			}
			
			foreach (List<Point> polyline in polylineGeometry.PolylinesCopy)
			{
				foreach (Point p in polyline)
				{
					if (!labelDictionary.GetAllPoints().Contains(p))
					{
						labelDictionary.Add(p, nextNewLabel);
						nextNewLabel++;
					}
				}
			}

			var connections = polylineGeometry.ConvertToConnections(labelDictionary);
		
			var newShape = new Shape(connections);
			Debug.Assert(newShape.ConformsWithGeometry(polylineGeometry, out _));
			newShapeLabeling = labelDictionary;
			return newShape;
		}

		/// <summary>
		/// Wheher the input is of this shape
		/// </summary>
		/// <param name="polylineGeometry"> the polyline geometry to check against this shape </param>
		/// <param name="labeling"> if the input is of this shape, output how each point in the polylines is labeled 
		/// (not guaranteed to be the only solution); otherwise output null </param>
		/// <returns> whether the intput is of this shape </returns>
		public bool ConformsWithGeometry(PolylinesGeometry polylineGeometry, out LabelingDictionary labeling)
		{
			try
			{
				labeling = this.SolveLabeling(polylineGeometry, null);
				return true;
			}
			catch (ShapeMatchFailureException)
			{
				labeling = null;
				return false;
			}
		}

		/// <summary>
		/// Find one way the geometry can conform with this shape, given a partial solution
		/// </summary>
		/// <returns> union of the solution with the input partial solution </returns>
		/// <exception cref="ShapeMatchFailureException"> throws when the input geometry is not of this shape </exception>
		public LabelingDictionary SolveLabeling(PolylinesGeometry polylineGeometry, LabelingDictionary partialLabelingSolution)
		{
			if (polylineGeometry is null)
			{
				throw new ArgumentNullException();
			}
			if (polylineGeometry.PolylinesCopy.Count == 0)
			{
				if (this.DefiningConnections.Count == 0)
				{
					return new LabelingDictionary();
				}
				else
				{
					throw new ShapeMatchFailureException("the shape definition is empty whereas the input geometry is not");
				}
			}

			// step1: find all unique points in the polylines and check if the count is the same as the count of labels in shape
			var uniqueCoordinates = new HashSet<Point>();
			foreach (List<Point> pl in polylineGeometry.PolylinesCopy)
			{
				uniqueCoordinates.UnionWith(pl);
			}
			if (uniqueCoordinates.Count != this.GetAllLabels().Count)
			{
				throw new ShapeMatchFailureException("input geometry has more unique points than there are labels in this shape");
			}

			// step2: generate the remaining coordinates and labels to match aside from the partial solution
			var coordinatesToWorkOn = new HashSet<Point>(uniqueCoordinates);
			if (partialLabelingSolution is object)
			{
				coordinatesToWorkOn.ExceptWith(partialLabelingSolution.GetAllPoints());
			}

			var labelsLeftToWorkOn = this.GetAllLabels();
			if (partialLabelingSolution is object)
			{
				labelsLeftToWorkOn.ExceptWith(partialLabelingSolution.GetAllLabels());
			}

			if (coordinatesToWorkOn.Count != labelsLeftToWorkOn.Count)
			{
				throw new ShapeMatchFailureException("remaining labels cannot map one to one with remaining unique corrdinates");
			}

			if (labelsLeftToWorkOn.Count == 0)
			{
				// the input is in fact a complete solution
				return partialLabelingSolution.Copy();
			}

			// step3: generate all potential ways each unique point can be labeled
			var coordinatesToWorkOnList = new List<Point>(coordinatesToWorkOn);
			var allPotentialLabelingForWhatsLeft = Utilities.GenerateAllPermutations(new List<int>(labelsLeftToWorkOn));

			// step4: check if there is one potential labeling with which the input would match the definition of this shape
			foreach (List<int> labelingInstanceForWhatsLeft in allPotentialLabelingForWhatsLeft)
			{
				Debug.Assert(coordinatesToWorkOn.Count == labelingInstanceForWhatsLeft.Count);

				LabelingDictionary labelDictionaryForAllPointsAndLabels;
				if (partialLabelingSolution is null)
				{
					labelDictionaryForAllPointsAndLabels = new LabelingDictionary();
				}
				else
				{
					labelDictionaryForAllPointsAndLabels = partialLabelingSolution.Copy();
				}

				for (int i = 0; i < coordinatesToWorkOnList.Count; i++)
				{
					var s = labelDictionaryForAllPointsAndLabels.Add(coordinatesToWorkOnList[i], labelingInstanceForWhatsLeft[i]);
					Debug.Assert(s);
				}

				var connections = polylineGeometry.ConvertToConnections(labelDictionaryForAllPointsAndLabels);

				if (this.DefiningConnections.SetEquals(connections))
				{
					return labelDictionaryForAllPointsAndLabels;
				}
			}
			throw new ShapeMatchFailureException("Failed to find a labeling that works");
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
			if (this.DefiningConnections.Count != s.DefiningConnections.Count)
			{
				return false;
			}

			foreach (Connection c in this.DefiningConnections)
			{
				if (! s.DefiningConnections.Contains(c))
				{
					return false;
				}
			}

			return true;
		}
	}

	class ShapeMatchFailureException : Exception 
	{
		public ShapeMatchFailureException() { }
		public ShapeMatchFailureException(string message) : base(message) { }
	}
}
