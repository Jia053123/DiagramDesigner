using BasicGeometries;
using ListOperations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShapeGrammarEngine
{
	class GrammarRule
	{
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

		/// <summary>
		/// Create a shape grammar rule given its both sides
		/// leftHandShape and rightHandShape share the same set of labels, so having the same label in both parameters means 
		/// the point corresponding to that node will stay the same when the rule is applied. Otherwise, the point is added or removed
		/// </summary>
		public GrammarRule(Shape leftHandShape, Shape rightHandShape)
		{
			this.LeftHandShape = leftHandShape;
			this.RightHandShape = rightHandShape;
		}

		/// <summary>
		/// Create a shape grammar rule given a single example of its application
		/// </summary>
		/// <param name="geometryBefore"> the group of polylines before the rule is applied </param>
		/// <param name="geometryAfter"> the group of polylines after the rule is applied </param>
		/// <param name="labeling"> outputs the labeling used in this creation </param>
		public static GrammarRule CreateGrammarRuleFromOneExample(PolylineGeometry geometryBefore, PolylineGeometry geometryAfter, out LabelingDictionary labeling)
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
		private bool ConformWithRule(PolylineGeometry geometryBefore, PolylineGeometry geometryAfter, out LabelingDictionary labeling)
		{
			if (!this.LeftHandShape.ConformsWithGeometry(geometryBefore, out _))
			{
				throw new ArgumentException("geometryBefore does not conform with ShapeBefore");
			}
			if (!this.RightHandShape.ConformsWithGeometry(geometryAfter, out _))
			{
				throw new ArgumentException("geometryAfter does not conform with ShapeAfter");
			}

			LabelingDictionary lDic = this.LeftHandShape.SolveLabeling(geometryBefore, null);
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
		public void LearnFromExample(PolylineGeometry geometryBefore, PolylineGeometry geometryAfter)
		{
			if (!this.LeftHandShape.ConformsWithGeometry(geometryBefore, out _))
			{
				throw new ArgumentException("geometryBefore does not conform with ShapeBefore");
			}
			if (!this.RightHandShape.ConformsWithGeometry(geometryAfter, out _))
			{
				throw new ArgumentException("geometryAfter does not conform with ShapeAfter");
			}

			LabelingDictionary labeling;
			var s = this.ConformWithRule(geometryBefore, geometryAfter, out labeling);
			if (!s)
			{
				throw new ArgumentException("geometries does not conform with this rule");
			}

			this.ApplicationRecords.Add(new RuleApplicationRecord(geometryBefore, geometryAfter, labeling));
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
		public PolylineGeometry ApplyToGeometry(PolylineGeometry polyGeo)
		{
			// Step1: check for conformity and label the input geometry
			LabelingDictionary labeling;
			var doesConform = this.LeftHandShape.ConformsWithGeometry(polyGeo, out labeling);
			if (! doesConform)
			{
				throw new ArgumentException("polylines does not conform with ShapeBefore");
			}

			var resultPolylines = new PolylineGeometry(polyGeo.PolylinesCopy);

			// Step2: remove the connections to be removed
			foreach (Connection c in this.ConnectionsToBeRemoved())
			{
				Point endPoint1 = labeling.GetPointByLabel(c.LabelOfFirstNode);
				Point endPoint2 = labeling.GetPointByLabel(c.LabelOfSecondNode);

				resultPolylines.EraseSegmentByPoints(endPoint1, endPoint2);
			}

			// Step3: add the connections to be added. If intersection happens, retry
			var connectionsToAdd = new Queue<Connection>(this.ConnectionsToBeAdded());
			bool progressMade;
			while (connectionsToAdd.Count > 0)
			{
				int beforeCount = connectionsToAdd.Count;
				this.AddConnectionsWithOneOrTwoExistingPoint(ref connectionsToAdd, labeling, ref resultPolylines);
				progressMade = beforeCount < connectionsToAdd.Count;
				if (!progressMade)
				{
					// TODO: define one connection using a hypothetical connection, allowing intersection. This one will be marked to be removed

				}
			}

			return resultPolylines;
		}

		private void AddConnectionsWithOneOrTwoExistingPoint( 
			ref Queue<Connection> connectionsToAdd, 
			LabelingDictionary labeling, 
			ref PolylineGeometry geometryToModify)
		{
			for (int i = 0; i < connectionsToAdd.Count; i++)
			{
				var newConnection = connectionsToAdd.Dequeue();
				if (this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfFirstNode) &&
					this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfSecondNode))
				{
					// both endpoints already exist: simply connect the existing points
					
					Point endpoint1 = labeling.GetPointByLabel(newConnection.LabelOfFirstNode);
					Point endpoint2 = labeling.GetPointByLabel(newConnection.LabelOfSecondNode);
					geometryToModify.AddSegmentByPoints(endpoint1, endpoint2);
				}
				else if (this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfFirstNode) && 
					!this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfSecondNode))
				{
					var labelForExistingPoint = newConnection.LabelOfFirstNode;
					var labelForPointToAssign = newConnection.LabelOfSecondNode;
					Point existingPoint = labeling.GetPointByLabel(labelForExistingPoint);
					var assignedPoint = this.AssignSecondPointForConnection(labeling, existingPoint, labelForExistingPoint, labelForPointToAssign);
					geometryToModify.AddSegmentByPoints(existingPoint, assignedPoint);
				}
				else if (!this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfFirstNode) && 
					this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfSecondNode))
				{
					var labelForExistingPoint = newConnection.LabelOfSecondNode;
					var labelForPointToAssign = newConnection.LabelOfFirstNode;

					Point existingPoint = labeling.GetPointByLabel(labelForExistingPoint);
					var assignedPoint = this.AssignSecondPointForConnection(labeling, existingPoint, labelForExistingPoint, labelForPointToAssign);
					geometryToModify.AddSegmentByPoints(existingPoint, assignedPoint);
				}
				else
				{
					// neither endpoints exist yet: leave it for later
					connectionsToAdd.Enqueue(newConnection);
				}
			}
		}

		private Point AssignSecondPointForConnection(LabelingDictionary leftHandGeometryLabeling, Point existingPoint, int labelForExistingPoint, int labelForPointToAssign)
		{
			var assignedAngle = this.AssignAngle(leftHandGeometryLabeling, labelForExistingPoint, labelForPointToAssign);
			var assignedLength = this.AssignLength(leftHandGeometryLabeling, labelForExistingPoint, labelForPointToAssign);

			var assignedPoint = LineSegment.LocateOtherEndPoint(existingPoint, assignedAngle, assignedLength);
			return assignedPoint;
		}

		internal double AssignAngle(LabelingDictionary leftHandGeometryLabeling, int labelForExistingPoint, int labelForPointToAssign)
		{
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
					Point pastExistingPoint = record.Labeling.GetPointByLabel(labelForExistingPoint);
					Point pastAssignedPoint = record.Labeling.GetPointByLabel(labelForPointToAssign);
					double pastAssignedAngle = pastExistingPoint.AngleTowardsPoint(pastAssignedPoint);

					var pointFrom = record.Labeling.GetPointByLabel(connection.LabelOfFirstNode);
					var pointTowards = record.Labeling.GetPointByLabel(connection.LabelOfSecondNode);
					var refAngle = pointFrom.AngleTowardsPoint(pointTowards);

					referenceAndAssignedValueSummaryForEachConnectionForEachRecord[i].Add((refAngle, pastAssignedAngle));
				}
				scoreForEachConnection[i] = GrammarRule.CalculateScoreForOneConnection(referenceAndAssignedValueSummaryForEachConnectionForEachRecord[i]);
			}

			// Step3: use the connection with the highest score as reference to assign this angle
			var chosenConnectionIndex = scoreForEachConnection.IndexOf(scoreForEachConnection.Max());
			var chosenConnection = connections[chosenConnectionIndex];
			var referencePointFrom = leftHandGeometryLabeling.GetPointByLabel(chosenConnection.LabelOfFirstNode);
			var referencePointTo = leftHandGeometryLabeling.GetPointByLabel(chosenConnection.LabelOfSecondNode);
			var referenceAngle = referencePointFrom.AngleTowardsPoint(referencePointTo);
			var referenceAndAssignedValueSummaryForChosenConnectionForEachRecord = referenceAndAssignedValueSummaryForEachConnectionForEachRecord[chosenConnectionIndex];

			var assignedAngle = GrammarRule.AssignValueBasedOnPastOccurances(referenceAngle, referenceAndAssignedValueSummaryForChosenConnectionForEachRecord);
			return assignedAngle;
		}


		internal double AssignLength(LabelingDictionary leftHandGeometryLabeling, int labelForExistingPoint, int labelForPointToAssign)
		{
			return -1; // stub
		}

		/// <summary>
		/// Calculate the score for one connection for the sake of choosing the best connection as the reference.
		/// The higher the score the better this connection works as the reference. 
		/// </summary>
		/// <returns> The evaluated score which may be negative. Higher means better.  </returns>
		internal static double CalculateScoreForOneConnection(List<(double referenceValue, double assignedValue)> pastData)
		{
			var differences = new List<double>();
			// calculate differences
			foreach ((double referenceValue, double assignedValue) data in pastData)
			{
				var difference = data.referenceValue - data.assignedValue;
				differences.Add(difference);
			}
			var variance = Utilities.CalculateVariance(differences);
			return variance * -1; // the lower the variance, the better this connection works as a reference
		}

		/// <summary>
		/// Taken the past reference values (from the chosen connection) and assigned values into consideration, 
		/// given the existing reference value, output the value to be assigned
		/// </summary>
		internal static double AssignValueBasedOnPastOccurances(double existingReferenceValue, List<(double referenceValue, double assignedValue)> pastData)
		{
			// figure out the range of ratio allowed
			double? minAssignedOverReferenceRatio = null;
			double? maxAssignedOverReferenceRatio = null;
			foreach ((double referenceValue, double assignedValue) entry in pastData)
			{
				var assignedOverReferenceRatio = entry.assignedValue / entry.referenceValue;

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
	}
}
