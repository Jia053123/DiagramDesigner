using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DiagramDesignerGeometryParser;

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
				labeling = this.SolveLabeling(polylineGeometry, null).First();
			}
			catch (ShapeMatchFailureException)
			{
				labeling = null;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Find all potential ways the geometry can conform with this shape, given a partial solution
		/// </summary>
		/// <returns> unions of each solution with the input partial solution. Guaranteed to have at least one item </returns>
		/// <exception cref="ShapeMatchFailureException"> throws when the input geometry is not of this shape </exception>
		public List<LabelingDictionary> SolveLabeling(PolylinesGeometry polylineGeometry, LabelingDictionary partialLabelingSolution)
		{
			if (polylineGeometry is null)
			{
				throw new ArgumentNullException();
			}
			if (polylineGeometry.IsEmpty())
			{
				if (this.DefiningConnections.Count == 0)
				{
					return new List<LabelingDictionary> { new LabelingDictionary() };
				}
				else
				{
					throw new ShapeMatchFailureException("the shape definition is empty whereas the input geometry is not");
				}
			}

			// step1: find all unique points in the polylines and check if the count is the same as the count of labels in shape
			var uniqueCoordinatesInGeo = new HashSet<Point>();
			foreach (List<Point> pl in polylineGeometry.PolylinesCopy)
			{
				uniqueCoordinatesInGeo.UnionWith(pl);
			}
			if (uniqueCoordinatesInGeo.Count != this.GetAllLabels().Count)
			{
				throw new ShapeMatchFailureException("input geometry has more unique points than there are labels in this shape");
			}

			// step2: generate the remaining coordinates and labels to match aside from the partial solution
			var labelsToWorkOn = this.GetAllLabels();
			if (partialLabelingSolution is object)
			{
				labelsToWorkOn.ExceptWith(partialLabelingSolution.GetAllLabels());
			}
			if (labelsToWorkOn.Count == 0)
			{
				// the input is in fact a complete solution and therefore the only solution
				return new List<LabelingDictionary> { partialLabelingSolution.Copy() };
			}

			List<LabelingDictionary> solutions = new List<LabelingDictionary>();
			foreach (int l in labelsToWorkOn)
			{
				var startingLabelingDic = partialLabelingSolution.Copy(); // create a new dic for each possible assignment for the starting Point
				Debug.Assert(!polylineGeometry.IsEmpty());
				startingLabelingDic.Add(polylineGeometry.GetPointByIndex(0,0), l);

				var startingLabelsToWorkOn = new HashSet<int>(labelsToWorkOn);
				startingLabelsToWorkOn.Remove(l);

				solutions.AddRange(this.SolveLabelingHelper(polylineGeometry, 0, 0, startingLabelsToWorkOn, startingLabelingDic));
			}
			return solutions;
		}

		/// <summary>
		/// Find all potential ways the geometry can conform with this shape, given a partial solution
		/// </summary>
		/// <param name="polylinesGeometryToSolve"></param>
		/// <param name="currentPointIndex"></param>
		/// <param name="currentPolylineIndex"></param>
		/// <param name="labelsLeftToWorkOn"></param>
		/// <param name="partialSolution"> cannot be null; if there is no partial solution this should be empty </param>
		/// <returns></returns>
		private List<LabelingDictionary> SolveLabelingHelper(PolylinesGeometry polylinesGeometryToSolve, int currentPointIndex, int currentPolylineIndex, HashSet<int> labelsLeftToWorkOn, LabelingDictionary partialSolution)
		{
			var nextIndexes = polylinesGeometryToSolve.FindIndexForNextPoint(currentPointIndex, currentPolylineIndex);
			if (nextIndexes.nextPointIndex != -1 && nextIndexes.nextPolylineIndex != -1)
			{
				// not done going through the whole geometry yet
				Point nextPoint = polylinesGeometryToSolve.GetPointByIndex(nextIndexes.nextPointIndex, nextIndexes.nextPolylineIndex);
				if (partialSolution.GetAllPoints().Contains(nextPoint))
				{
					// but the next point is already assigned
					return SolveLabelingHelper(polylinesGeometryToSolve, nextIndexes.nextPointIndex, nextIndexes.nextPolylineIndex, labelsLeftToWorkOn, partialSolution); 
				}

				if (currentPolylineIndex == nextIndexes.nextPolylineIndex)
				{
					// still in the same polyline, so the current point and next point are connected
					Point currentPoint = polylinesGeometryToSolve.GetPointByIndex(currentPolylineIndex, currentPointIndex);
					int currentLabel = partialSolution.GetLabelByPoint(currentPoint);
					var connectedLabels = this.LabelsConnectedTo(currentLabel);
					List<LabelingDictionary> solutions = new List<LabelingDictionary>();
					foreach (int l in connectedLabels)
					{
						if (!labelsLeftToWorkOn.Contains(l))
						{
							continue; // this label is already assigned; skip
						}
						// perform assignment of nextPoint to l
						var moreCompleteSolution = partialSolution.Copy();
						moreCompleteSolution.Add(nextPoint, l);
						var updatedLabelsLeftToWorkOn = new HashSet<int>(labelsLeftToWorkOn);
						updatedLabelsLeftToWorkOn.Remove(l);
						solutions.AddRange(this.SolveLabelingHelper(polylinesGeometryToSolve, nextIndexes.nextPointIndex, nextIndexes.nextPolylineIndex, updatedLabelsLeftToWorkOn, moreCompleteSolution));
					}
					return solutions;
				}
				else
				{
					// end of the polyline, but there are still more polylines
					// assign random remaining labels to the next point
					List<LabelingDictionary> solutions = new List<LabelingDictionary>();
					foreach (int l in labelsLeftToWorkOn)
					{
						// perform assignment of nextPoint to l
						var moreCompleteSolution = partialSolution.Copy();
						moreCompleteSolution.Add(nextPoint, l);
						var updatedLabelsLeftToWorkOn = new HashSet<int>(labelsLeftToWorkOn);
						updatedLabelsLeftToWorkOn.Remove(l);
						solutions.AddRange(this.SolveLabelingHelper(polylinesGeometryToSolve, nextIndexes.nextPointIndex, nextIndexes.nextPolylineIndex, updatedLabelsLeftToWorkOn, moreCompleteSolution));
					}
					return solutions;
				}
			}
			else
			{
				// done with the polylinesGeometry
				Debug.Assert(labelsLeftToWorkOn.Count == 0);
				return new List<LabelingDictionary>{partialSolution};
			}
		}

		/// <summary>
		/// find the labels connected to the input in this shape definition
		/// </summary>
		/// <param name="label"> the label to search for </param>
		/// <returns> the list of labels connected to the input according to the shape definition. If no labels are found, return an empty list </returns>
		/// <exception cref="ArgumentException"> throws if the input label is not part of the shape definition </exception>
		internal List<int> LabelsConnectedTo(int label)
		{
			if (!this.GetAllLabels().Contains(label))
			{
				throw new ArgumentException();
			}

			var labelsConnectedTo = new List<int>();
			foreach (Connection connection in this.DefiningConnections)
			{
				if (connection.LabelOfFirstNode == label)
				{
					labelsConnectedTo.Add(connection.LabelOfSecondNode);
				}
				else if (connection.LabelOfSecondNode == label)
				{
					labelsConnectedTo.Add(connection.LabelOfFirstNode);
				}
			}

			return labelsConnectedTo;
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
