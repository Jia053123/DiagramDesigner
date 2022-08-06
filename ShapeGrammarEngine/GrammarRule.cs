using BasicGeometries;
using ListOperations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShapeGrammarEngine
{
	public class GrammarRule : IEquatable<GrammarRule>
	{
		/// <summary>
		/// The ID that's unique for each GrammarRule object created
		/// </summary>
		public readonly Guid id;

		/// <summary>
		/// The shape before the rule is applied. It shares labels with RightHandShape
		/// </summary>
		public Shape LeftHandShape { get; private set; }
		/// <summary>
		/// The shape on the right hand side of the rule. It shares labels with ShapeBefore  
		/// </summary>
		public Shape RightHandShape { get; private set; }

		public List<Connection> ConnectionsErased { get; private set; }

		/// <summary>
		/// Make sure all the new line segments created by this rule is orthogonal
		/// </summary>
		public bool DrawNewLinesOrthogonally { get; private set; } = true;

		/// <summary>
		/// Make sure the spaces created by this rule is acceptable for at least one program requirement
		/// </summary>
		public bool CheckForMinimumProgramSize { get; private set; } = true;

		public bool IsATerminalRule { get; private set; }

		private static Random RandomGenerator = new Random(Environment.TickCount);

		internal List<RuleApplicationRecord> ApplicationRecords = new List<RuleApplicationRecord>();
		public int SampleCount => this.ApplicationRecords.Count;

		/// <summary>
		/// Create a shape grammar rule given its both sides
		/// leftHandShape and rightHandShape share the same set of labels, so having the same label in both parameters means 
		/// the point corresponding to that node will stay the same when the rule is applied. Otherwise, the point is added or removed
		/// </summary>
		public GrammarRule(Shape leftHandShape, Shape rightHandShape)
		{
			this.LeftHandShape = leftHandShape;
			this.RightHandShape = rightHandShape;
			this.id = Guid.NewGuid();
		}

		/// <summary>
		/// Create a shape grammar rule given a single example of its application
		/// </summary>
		/// <param name="geometryBefore"> the group of polylines before the rule is applied </param>
		/// <param name="geometryAfter"> the group of polylines after the rule is applied </param>
		/// <param name="labeling"> outputs the labeling used in this creation </param>
		public static GrammarRule CreateGrammarRuleFromOneExample(PolylinesGeometry geometryBefore, PolylinesGeometry geometryAfter, out LabelingDictionary labeling)
		{
			LabelingDictionary lhsLabeling, sharedLabeling;
			var lhs = Shape.CreateShapeFromPolylines(geometryBefore, null, out lhsLabeling);
			var rhs = Shape.CreateShapeFromPolylines(geometryAfter, lhsLabeling, out sharedLabeling);

			labeling = sharedLabeling;
			var newRule = new GrammarRule(lhs, rhs);
			newRule.ApplicationRecords.Add(new RuleApplicationRecord(geometryBefore, geometryAfter, labeling));
			return newRule;
		}

		/// <summary>
		/// Whether the input geometries are consistent with the rule
		/// </summary>
		/// <param name="geometryBefore"> the geometry before the rule is applied </param>
		/// <param name="geometryAfter"> the geometry after the rule is applied </param>
		/// <param name="labeling"> if the input geometries are consistent with the rule, 
		/// how their line segments map onto connections of the shapes in the rule; otherwise output null </param>
		/// <returns> whether the input geometries are consistent with the rule </returns>
		public bool DoesConformWithRule(PolylinesGeometry geometryBefore, PolylinesGeometry geometryAfter, out LabelingDictionary labeling)
		{
			if (!this.LeftHandShape.ConformsWithGeometry(geometryBefore, out _))
			{
				labeling = null;
				return false;
			}
			if (!this.RightHandShape.ConformsWithGeometry(geometryAfter, out _))
			{
				labeling = null;
				return false;
			}

			LabelingDictionary lDic = this.LeftHandShape.SolveLabeling(geometryBefore, null); // this may not be unique!!
			LabelingDictionary sharedDic;
			try
			{
				sharedDic = this.RightHandShape.SolveLabeling(geometryAfter, lDic);
			}
			catch (ShapeMatchFailureException)
			{
				labeling = null;
				return false;
			}

			labeling = sharedDic;
			return true;
		}

		/// <summary>
		/// Learn how the rule can be applied from examples in terms of proportions. 
		/// </summary>
		/// <param name="geometryBefore"> The geometry in the example before the rule is applied </param>
		/// <param name="geometryAfter"> The geometry in the example after the rule is applied </param>
		/// <exception cref="ShapeGrammarEngine.GeometryParsingFailureException"> 
		/// Thrown when geometryBefore and geometryAfter cannot be parsed using the rule </exception>
		public void LearnFromExample(PolylinesGeometry geometryBefore, PolylinesGeometry geometryAfter, out LabelingDictionary labeling)
		{
			if (!this.LeftHandShape.ConformsWithGeometry(geometryBefore, out _))
			{
				throw new GeometryParsingFailureException("geometryBefore does not conform with ShapeBefore");
			}
			if (!this.RightHandShape.ConformsWithGeometry(geometryAfter, out _))
			{
				throw new GeometryParsingFailureException("geometryAfter does not conform with ShapeAfter");
			}

			LabelingDictionary l;
			var s = this.DoesConformWithRule(geometryBefore, geometryAfter, out l);
			if (!s)
			{
				throw new GeometryParsingFailureException("geometries does not conform with this rule");
			}

			this.ApplicationRecords.Add(new RuleApplicationRecord(geometryBefore, geometryAfter, l));
			labeling = l;
		}

		private HashSet<Connection> ConnectionsToBeRemoved()
		{
			var cbuptbr = new HashSet<Connection>(this.LeftHandShape.DefiningConnections);
			cbuptbr.ExceptWith(this.RightHandShape.DefiningConnections);
			return cbuptbr;
		}

		private HashSet<Connection> ConnectionsToBeAdded()
		{
			var cbuptba = new HashSet<Connection>(this.RightHandShape.DefiningConnections);
			cbuptba.ExceptWith(this.LeftHandShape.DefiningConnections);
			return cbuptba;
		}

		/// <summary>
		/// Apply the rule with knowledge learnt from examples. 
		/// </summary>
		/// <param name="polyGeo"> the geometry on which the rule will be applied. It must confrom with LeftHandShape </param>
		/// <returns> the geometry after the rule is applied. It will confrom with RightHandShape </returns>
		public PolylinesGeometry ApplyToGeometry(PolylinesGeometry polyGeo)
		{
			if (this.ApplicationRecords.Count == 0)
			{
				throw new RuleApplicationFailureException("Cannot apply without history to learn from");
			}

			// Step1: check for conformity and label the input geometry
			LabelingDictionary labeling;
			var doesConform = this.LeftHandShape.ConformsWithGeometry(polyGeo, out labeling);
			if (! doesConform)
			{
				throw new GeometryParsingFailureException("polylines does not conform with ShapeBefore");
			}

			var resultPolylines = new PolylinesGeometry(polyGeo.PolylinesCopy);

			// Step2: add the connections to be added.
			if (this.ConnectionsToBeAdded().Count > 0)
			{
				this.AddConnections(this.ConnectionsToBeAdded(), ref resultPolylines, ref labeling);
			}

			// Step3: remove the connections to be removed
			foreach (Connection c in this.ConnectionsToBeRemoved())
			{
				Point endPoint1 = labeling.GetPointByLabel(c.LabelOfFirstNode);
				Point endPoint2 = labeling.GetPointByLabel(c.LabelOfSecondNode);

				resultPolylines.EraseSegmentByPoints(endPoint1, endPoint2);
			}

			// TODO: check for intersections and overlaps?

			return resultPolylines;
		}

		/// <summary>
		/// Add connections if there are at least one end point already in geometry
		/// </summary>
		/// <param name="connectionsToAdd"> The connections to try adding. Any unadded connection would remain after this returns </param>
		/// <param name="geometryToModify"> The geometry that's being modified.  </param>
		/// <param name="labelingForGeometryToModify"></param>
		private void AddConnections(HashSet<Connection> connectionsToAdd, ref PolylinesGeometry geometryToModify, ref LabelingDictionary labelingForGeometryToModify)
		{
			if (!geometryToModify.GetAllPoints().IsSubsetOf(labelingForGeometryToModify.GetAllPoints()))
			{
				throw new ArgumentException("labelingForGeometryToModify does not cover every point in geometryToModify");
			}
			var connectionsToAddQueue = new Queue<Connection>(connectionsToAdd);

			while (connectionsToAddQueue.Count > 0)
			{
				int beforeQueueCount = connectionsToAddQueue.Count;
				for (int i = 0; i < connectionsToAddQueue.Count; i++)
				{
					var connectionToAdd = connectionsToAddQueue.Dequeue();
					if (labelingForGeometryToModify.GetAllLabels().Contains(connectionToAdd.LabelOfFirstNode) &&
						labelingForGeometryToModify.GetAllLabels().Contains(connectionToAdd.LabelOfSecondNode))
					{
						// both endpoints already exist: simply connect the existing points
						Point endpoint1 = labelingForGeometryToModify.GetPointByLabel(connectionToAdd.LabelOfFirstNode);
						Point endpoint2 = labelingForGeometryToModify.GetPointByLabel(connectionToAdd.LabelOfSecondNode);

						geometryToModify.AddSegmentByPoints(endpoint1, endpoint2);
					}
					else if (labelingForGeometryToModify.GetAllLabels().Contains(connectionToAdd.LabelOfFirstNode) &&
						!labelingForGeometryToModify.GetAllLabels().Contains(connectionToAdd.LabelOfSecondNode))
					{
						// only endpoint of the first label exists
						var labelForExistingPoint = connectionToAdd.LabelOfFirstNode;
						var labelForPointToAssign = connectionToAdd.LabelOfSecondNode;

						Point existingPoint = labelingForGeometryToModify.GetPointByLabel(labelForExistingPoint);
						var assignedPoint = this.AssignNewPointByLearning(labelingForGeometryToModify, labelForExistingPoint, labelForPointToAssign);

						geometryToModify.AddSegmentByPoints(existingPoint, assignedPoint);
						var s = labelingForGeometryToModify.Add(assignedPoint, labelForPointToAssign);
						Debug.Assert(s);
					}
					else if (!labelingForGeometryToModify.GetAllLabels().Contains(connectionToAdd.LabelOfFirstNode) &&
						labelingForGeometryToModify.GetAllLabels().Contains(connectionToAdd.LabelOfSecondNode))
					{
						// only endpoint of the second label exists
						var labelForExistingPoint = connectionToAdd.LabelOfSecondNode;
						var labelForPointToAssign = connectionToAdd.LabelOfFirstNode;

						Point existingPoint = labelingForGeometryToModify.GetPointByLabel(labelForExistingPoint);
						var assignedPoint = this.AssignNewPointByLearning(labelingForGeometryToModify, labelForExistingPoint, labelForPointToAssign);

						geometryToModify.AddSegmentByPoints(existingPoint, assignedPoint);
						var s = labelingForGeometryToModify.Add(assignedPoint, labelForPointToAssign);
						Debug.Assert(s);
					}
					else
					{
						// neither endpoints exist yet: leave it for later
						connectionsToAddQueue.Enqueue(connectionToAdd);
					}
				}

				var progressMade = beforeQueueCount > connectionsToAddQueue.Count;
				if (!progressMade)
				{
					// every remaining connections to add have no existing endpoint
					var connectionToAdd = connectionsToAddQueue.Dequeue();

					// Step1: Assign the first node of the connection as if there is a connection between it and one existing point
					Debug.Assert(labelingForGeometryToModify.Count > 0);
					var labelForAExistingPoint = labelingForGeometryToModify.GetAllLabels().First(); // just pick a random existing point
					var labelForThePointOfFirstNode = connectionToAdd.LabelOfFirstNode;

					//Point existingPoint = labelingForGeometryToModify.GetPointByLabel(labelForAExistingPoint);
					Point assignedPointOfFirstNode = this.AssignNewPointByLearning(labelingForGeometryToModify, labelForAExistingPoint, labelForThePointOfFirstNode);

					// Step2: Add the assignedPointOfFirstNode and its label into the labelForExistingPoint in advance
					var s1 = labelingForGeometryToModify.Add(assignedPointOfFirstNode, labelForThePointOfFirstNode);
					Debug.Assert(s1);

					// Step3: Assign the second node of the connection given the assigned first node
					var labelForThePointOfSecondNode = connectionToAdd.LabelOfSecondNode;
					Point assignedPointOfSecondNode = this.AssignNewPointByLearning(labelingForGeometryToModify, labelForThePointOfFirstNode, labelForThePointOfSecondNode);

					geometryToModify.AddSegmentByPoints(assignedPointOfFirstNode, assignedPointOfSecondNode);
					var s2 = labelingForGeometryToModify.Add(assignedPointOfSecondNode, labelForThePointOfSecondNode);
					Debug.Assert(s2);
				}
			}
		}

		private Point AssignNewPointByLearning(LabelingDictionary NewGeometryLabeling, int labelForExistingPoint, int labelForPointToAssign)
		{
			var existingPoint = NewGeometryLabeling.GetPointByLabel(labelForExistingPoint);
			var assignedAngle = this.AssignAngle(NewGeometryLabeling, labelForExistingPoint, labelForPointToAssign);
			var assignedLength = this.AssignLength(NewGeometryLabeling, labelForExistingPoint, labelForPointToAssign);

			var assignedPoint = LineSegment.LocateOtherEndPoint(existingPoint, assignedAngle, assignedLength);
			return assignedPoint;
		}

		/// <summary>
		/// Assign the angle between any two points in right hand shape based on application records. 
		/// The two points do not have to form a connection
		/// </summary>
		/// <param name="newGeometryLabeling"> Labeling for the geometry currently being modified. May contain labels from both left hand and right hand shape </param>
		/// <param name="labelForPointFrom"> The label for the point from which the angle is calculated </param>
		/// <param name="labelForPointTowards"> The label for the point towards which the angle is calculated </param>
		/// <returns> The angle assigned is between -Pi and Pi </returns>
		internal double AssignAngle(LabelingDictionary newGeometryLabeling, int labelForPointFrom, int labelForPointTowards)
		{
			var allLabelsInShapes = this.LeftHandShape.GetAllLabels();
			allLabelsInShapes.UnionWith(this.RightHandShape.GetAllLabels());
			if (!newGeometryLabeling.GetAllLabels().IsSubsetOf(allLabelsInShapes))
			{
				throw new ArgumentException("newGeometryLabeling contains labels not in this rule");
			}
			if (! this.RightHandShape.GetAllLabels().Contains(labelForPointFrom))
			{
				throw new ArgumentException("labelForExistingPoint not in right hand shape");
			}
			if (!this.RightHandShape.GetAllLabels().Contains(labelForPointTowards))
			{
				throw new ArgumentException("labelForPointToAssign not in right hand shape");
			}

			// Step1: make a list of scores for each connection 
			var connections = new List<Connection>(this.LeftHandShape.DefiningConnections);
			var scoreForEachConnection = new List<double>();
			foreach (Connection _ in connections)
			{
				scoreForEachConnection.Add(0);
			}

			// Step2: go through application history to assign a score for each connection
			var referenceAndAssignedValueSummaryForEachConnectionForEachRecord = new List<List<(double referenceValue, double assignedValue)>>();
			for (int i = 0; i < connections.Count; i++)
			{
				var connection = connections[i];
				referenceAndAssignedValueSummaryForEachConnectionForEachRecord.Add(new List<(double, double)>());
				foreach (RuleApplicationRecord record in this.ApplicationRecords)
				{
					// get angle of assigned connection in this record
					Point pastExistingPoint = record.Labeling.GetPointByLabel(labelForPointFrom);
					Point pastAssignedPoint = record.Labeling.GetPointByLabel(labelForPointTowards);
					double pastAssignedAngle = pastExistingPoint.AngleTowardsPoint(pastAssignedPoint);

					// get angle of potential reference connection
					var pointFrom = record.Labeling.GetPointByLabel(connection.LabelOfFirstNode);
					var pointTowards = record.Labeling.GetPointByLabel(connection.LabelOfSecondNode);
					var refAngle = pointFrom.AngleTowardsPoint(pointTowards);

					referenceAndAssignedValueSummaryForEachConnectionForEachRecord[i].Add((refAngle, pastAssignedAngle));
				}
				scoreForEachConnection[i] = GrammarRule.CalculateScoreForOneConnectionByDifference(referenceAndAssignedValueSummaryForEachConnectionForEachRecord[i]);
			}

			// Step3: use the connection with the highest score as reference to assign this angle
			var chosenConnectionIndex = scoreForEachConnection.IndexOf(scoreForEachConnection.Max());
			var chosenConnection = connections[chosenConnectionIndex];
			var referencePointFrom = newGeometryLabeling.GetPointByLabel(chosenConnection.LabelOfFirstNode);
			var referencePointTo = newGeometryLabeling.GetPointByLabel(chosenConnection.LabelOfSecondNode);
			var referenceAngle = referencePointFrom.AngleTowardsPoint(referencePointTo);
			var referenceAndAssignedValueSummaryForChosenConnectionForEachRecord = referenceAndAssignedValueSummaryForEachConnectionForEachRecord[chosenConnectionIndex];

			var assignedAngle = GrammarRule.AssignValueBasedOnPastOccurancesByDifference(referenceAngle, referenceAndAssignedValueSummaryForChosenConnectionForEachRecord);
			while (assignedAngle > Math.PI)
			{
				assignedAngle -= Math.PI * 2;
			}
			while (assignedAngle < -1 * Math.PI)
			{
				assignedAngle += Math.PI * 2;
			}
			return assignedAngle;
		}

		/// <summary>
		/// Assign the length between any two points in right hand shape based on application records. 
		/// The two points do not have to form a connection
		/// </summary>
		/// <param name="newGeometryLabeling"> Labeling for the geometry currently being modified. May contain labels from both left hand and right hand shape </param>
		/// <param name="labelForPoint1"> The label for the point from which the angle is calculated </param>
		/// <param name="labelForPoint2"> The label for the point towards which the angle is calculated </param>
		/// <returns> The assigned length is always positive </returns>
		internal double AssignLength(LabelingDictionary newGeometryLabeling, int labelForPoint1, int labelForPoint2)
		{
			var allLabelsInShapes = this.LeftHandShape.GetAllLabels();
			allLabelsInShapes.UnionWith(this.RightHandShape.GetAllLabels());
			if (!newGeometryLabeling.GetAllLabels().IsSubsetOf(allLabelsInShapes))
			{
				throw new ArgumentException("newGeometryLabeling contains labels not in this rule");
			}
			if (!this.RightHandShape.GetAllLabels().Contains(labelForPoint1))
			{
				throw new ArgumentException("labelForExistingPoint not in right hand shape");
			}
			if (!this.RightHandShape.GetAllLabels().Contains(labelForPoint2))
			{
				throw new ArgumentException("labelForPointToAssign not in right hand shape");
			}

			// Step1: make a list of scores for each connection 
			var connections = new List<Connection>(this.LeftHandShape.DefiningConnections);
			var scoreForEachConnection = new List<double>();
			foreach (Connection _ in connections)
			{
				scoreForEachConnection.Add(0);
			}

			// Step2: go through application history to assign a score for each connection
			var referenceAndAssignedValueSummaryForEachConnectionForEachRecord = new List<List<(double referenceValue, double assignedValue)>>();
			for (int i = 0; i < connections.Count; i++)
			{
				var connection = connections[i];
				referenceAndAssignedValueSummaryForEachConnectionForEachRecord.Add(new List<(double, double)>());
				foreach (RuleApplicationRecord record in this.ApplicationRecords)
				{
					// get length of assigned connection in this record
					Point pastExistingPoint = record.Labeling.GetPointByLabel(labelForPoint1);
					Point pastAssignedPoint = record.Labeling.GetPointByLabel(labelForPoint2);
					double pastAssignedLength = Point.DistanceBetweenPoints(pastExistingPoint, pastAssignedPoint);
					Debug.Assert(pastAssignedLength > 0);

					// get length of potential reference connection
					var pointFrom = record.Labeling.GetPointByLabel(connection.LabelOfFirstNode);
					var pointTowards = record.Labeling.GetPointByLabel(connection.LabelOfSecondNode);
					var refLength = Point.DistanceBetweenPoints(pointFrom, pointTowards);
					Debug.Assert(refLength > 0);

					referenceAndAssignedValueSummaryForEachConnectionForEachRecord[i].Add((refLength, pastAssignedLength));
				}
				scoreForEachConnection[i] = GrammarRule.CalculateScoreForOneConnectionByRatio(referenceAndAssignedValueSummaryForEachConnectionForEachRecord[i]);
			}

			// Step3: use the connection with the highest score as reference to assign this angle
			var chosenConnectionIndex = scoreForEachConnection.IndexOf(scoreForEachConnection.Max());
			var chosenConnection = connections[chosenConnectionIndex];
			var referencePointFrom = newGeometryLabeling.GetPointByLabel(chosenConnection.LabelOfFirstNode);
			var referencePointTo = newGeometryLabeling.GetPointByLabel(chosenConnection.LabelOfSecondNode);
			var referenceLength = Point.DistanceBetweenPoints(referencePointFrom, referencePointTo);
			Debug.Assert(referenceLength > 0);
			var referenceAndAssignedValueSummaryForChosenConnectionForEachRecord = referenceAndAssignedValueSummaryForEachConnectionForEachRecord[chosenConnectionIndex];

			var assignedLength = GrammarRule.AssignValueBasedOnPastOccurancesByRatio(referenceLength, referenceAndAssignedValueSummaryForChosenConnectionForEachRecord);
			Debug.Assert(assignedLength > 0);
			return assignedLength;
		}


		/// <summary>
		/// Calculate the score for one connection for the sake of choosing the best connection as the reference.
		/// The higher the score the better this connection works as the reference. 
		/// </summary>
		/// <returns> The evaluated score which is equal to the negative of the variance 
		/// of the difference between the reference and assigned values across history.  </returns>
		internal static double CalculateScoreForOneConnectionByDifference(List<(double referenceValue, double assignedValue)> pastData)
		{
			var pastDifferences = new List<double>();
			// calculate differences
			foreach ((double referenceValue, double assignedValue) data in pastData)
			{
				var difference = data.assignedValue - data.referenceValue;
				pastDifferences.Add(difference);
			}
			var variance = Utilities.CalculateVariance(pastDifferences);
			return variance * -1; // the lower the variance, the better this connection works as a reference
		}

		/// <summary>
		/// Calculate the score for one connection for the sake of choosing the best connection as the reference.
		/// The higher the score the better this connection works as the reference. 
		/// </summary>
		/// <returns> The evaluated score which is equal to the negative of the variance 
		/// of the ratio between the reference and assigned values across history.  </returns>
		internal static double CalculateScoreForOneConnectionByRatio(List<(double referenceValue, double assignedValue)> pastData)
		{
			var pastRatios = new List<double>();
			// calculate differences
			foreach ((double referenceValue, double assignedValue) data in pastData)
			{
				if (data.referenceValue == 0)
				{
					throw new ArgumentException("One of the referenceValue is zero");
				}
				var ratio = data.assignedValue / data.referenceValue;
				pastRatios.Add(ratio);
			}
			var variance = Utilities.CalculateVariance(pastRatios);
			return variance * -1; // the lower the variance, the better this connection works as a reference
		}

		/// <summary>
		/// Taken the past reference values (from the chosen connection) and assigned values into consideration, 
		/// given the existing reference value, output the value to be assigned
		/// </summary>
		/// <param name="existingReferenceValue"> the reference value from which the value is assigned </param>
		/// <param name="pastData"> referenceValue item can be zero </param>
		internal static double AssignValueBasedOnPastOccurancesByDifference(double existingReferenceValue, List<(double referenceValue, double assignedValue)> pastData)
		{
			// figure out the range of difference allowed
			double? minAssignedMinusReferenceDiff = null;
			double? maxAssignedMinusReferenceDiff = null;
			foreach ((double referenceValue, double assignedValue) entry in pastData)
			{
				var assignedMinusReferenceDiff = entry.assignedValue - entry.referenceValue;

				if ((minAssignedMinusReferenceDiff is null) || (assignedMinusReferenceDiff < minAssignedMinusReferenceDiff))
				{
					minAssignedMinusReferenceDiff = assignedMinusReferenceDiff;
				}

				if ((maxAssignedMinusReferenceDiff is null) || (assignedMinusReferenceDiff > maxAssignedMinusReferenceDiff))
				{
					maxAssignedMinusReferenceDiff = assignedMinusReferenceDiff;
				} 
			}

			// assign with a random ratio in range
			double differenceToUse = (double)(GrammarRule.RandomGenerator.NextDouble() * (maxAssignedMinusReferenceDiff - minAssignedMinusReferenceDiff) + minAssignedMinusReferenceDiff);
			return differenceToUse + existingReferenceValue;
		}

		/// <summary>
		/// Taken the past reference values (from the chosen connection) and assigned values into consideration, 
		/// given the existing reference value, output the value to be assigned
		/// </summary>
		/// <param name="existingReferenceValue"> the reference value from which the value is assigned </param>
		/// <param name="pastData"> referenceValue item cannot be zero </param>
		internal static double AssignValueBasedOnPastOccurancesByRatio(double existingReferenceValue, List<(double referenceValue, double assignedValue)> pastData)
		{
			// figure out the range of ratio allowed
			double? minAssignedOverReferenceRatio = null;
			double? maxAssignedOverReferenceRatio = null;
			foreach ((double referenceValue, double assignedValue) entry in pastData)
			{
				if (entry.referenceValue == 0)
				{
					throw new ArgumentException("one of the referenceValue is zero");
				}
				
				var	assignedOverReferenceRatio = entry.assignedValue / entry.referenceValue;
				
				

				if ((minAssignedOverReferenceRatio is null) || (assignedOverReferenceRatio < minAssignedOverReferenceRatio))
				{
					minAssignedOverReferenceRatio = assignedOverReferenceRatio;
				}

				if ((maxAssignedOverReferenceRatio is null) || (assignedOverReferenceRatio > maxAssignedOverReferenceRatio))
				{
					maxAssignedOverReferenceRatio = assignedOverReferenceRatio;
				}
			}

			// assign with a random ratio in range
			double ratioToUse = (double)(GrammarRule.RandomGenerator.NextDouble() * (maxAssignedOverReferenceRatio - minAssignedOverReferenceRatio) + minAssignedOverReferenceRatio);
			return ratioToUse * existingReferenceValue;
		}

		public override bool Equals(object obj) => this.Equals(obj as GrammarRule);

		public bool Equals(GrammarRule other)
		{
			return this.id == other.id;
		}

		public static bool operator ==(GrammarRule lhs, GrammarRule rhs)
		{
			if (lhs is null)
			{
				if (rhs is null)
				{
					return true;
				}
				return false;
			}
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GrammarRule lhs, GrammarRule rhs) => !(lhs == rhs);

		public override int GetHashCode() => this.id.GetHashCode();
	}

	/// <summary>
	/// Thrown when unable to parse a geometry using a shape or left hand and right hand geometries using a rule
	/// </summary>
	public class GeometryParsingFailureException : Exception
	{
		public GeometryParsingFailureException() { }
		public GeometryParsingFailureException(string message) : base(message) { }
	}

	/// <summary>
	/// Thrown when encountering problem applying rules to left hand shape
	/// </summary>
	public class RuleApplicationFailureException : Exception
	{
		public RuleApplicationFailureException() { }
		public RuleApplicationFailureException(string message) : base(message) { }
	}
}
