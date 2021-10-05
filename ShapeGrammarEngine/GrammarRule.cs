using BasicGeometries;
using ListOperations;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

			LabelingDictionary lDic;
			_ = this.LeftHandShape.ConformsWithGeometry(geometryBefore, out lDic);

			labeling = null; // stub
			return false; // stub
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
			_ = GrammarRule.CreateGrammarRuleFromOneExample(geometryBefore, geometryAfter, out labeling); // TODO: need to guarantee the labeling is consistant with the two shapes!

			this.ApplicationRecords.Add(new RuleApplicationRecord(geometryBefore, geometryAfter, labeling));
		}

		private HashSet<Connection> ConnectionsToBeRemoved()
		{
			var cbuptbr = new HashSet<Connection>(this.LeftHandShape.Definition);
			cbuptbr.ExceptWith(this.RightHandShape.Definition);
			return cbuptbr;
		}

		private HashSet<Connection> ConnectionsToBeAdded()
		{
			var cbuptba = new HashSet<Connection>(this.RightHandShape.Definition);
			cbuptba.ExceptWith(this.LeftHandShape.Definition);
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

				polyGeo.EraseSegmentByPoints(endPoint1, endPoint2);
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

		private void AddConnectionsWithOneOrTwoExistingPoint(ref Queue<Connection> connectionsToAdd, LabelingDictionary reverseLabeling, ref PolylineGeometry geometryToModify)
		{
			for (int i = 0; i < connectionsToAdd.Count; i++)
			{
				var newConnection = connectionsToAdd.Dequeue();
				if (this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfFirstNode) &&
					this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfSecondNode))
				{
					// both endpoints already exist: simply connect the existing points
					
					Point endpoint1 = reverseLabeling.GetPointByLabel(newConnection.LabelOfFirstNode);
					Point endpoint2 = reverseLabeling.GetPointByLabel(newConnection.LabelOfSecondNode);
					geometryToModify.AddSegmentByPoints(endpoint1, endpoint2);
				}
				else if (this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfFirstNode) && 
					!this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfSecondNode))
				{
					var labelForExistingPoint = newConnection.LabelOfFirstNode;
					var labelForPointToAssign = newConnection.LabelOfSecondNode;
					Point existingPoint = reverseLabeling.GetPointByLabel(labelForExistingPoint);
					var assignedPoint = this.AssignSecondPointForConnection(existingPoint, labelForExistingPoint, labelForPointToAssign);
					geometryToModify.AddSegmentByPoints(existingPoint, assignedPoint);
				}
				else if (!this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfFirstNode) && 
					this.LeftHandShape.GetAllLabels().Contains(newConnection.LabelOfSecondNode))
				{
					var labelForExistingPoint = newConnection.LabelOfSecondNode;
					var labelForPointToAssign = newConnection.LabelOfFirstNode;

					Point existingPoint = reverseLabeling.GetPointByLabel(labelForExistingPoint);
					var assignedPoint = this.AssignSecondPointForConnection(existingPoint, labelForExistingPoint, labelForPointToAssign);
					geometryToModify.AddSegmentByPoints(existingPoint, assignedPoint);
				}
				else
				{
					// neither endpoints exist yet: leave it for later
					connectionsToAdd.Enqueue(newConnection);
				}
			}
		}

		private Point AssignSecondPointForConnection(Point existingPoint, int labelForExistingPoint, int labelForPointToAssign)
		{
			// Step1: find all past occurences of the two endpoints
			var pastLeftHandGeometries = new List<PolylineGeometry>();
			var pastExistingPoints = new List<Point>();
			var pastAssignedPoints = new List<Point>();
			foreach (RuleApplicationRecord rar  in this.ApplicationRecords)
			{
				pastLeftHandGeometries.Add(rar.GeometryBefore);
				
				Point pastExistingPoint = rar.Labeling.GetPointByLabel(labelForExistingPoint);
				Point pastAssignedPoint = rar.Labeling.GetPointByLabel(labelForPointToAssign);
				pastExistingPoints.Add(pastExistingPoint);
				pastAssignedPoints.Add(pastAssignedPoint);
			}

			// Step2: assign angle and length
			var assignedAngle = GrammarRule.AssignAngle(existingPoint, pastLeftHandGeometries, pastExistingPoints, pastAssignedPoints);
			var assignedLength = GrammarRule.AssignLength(existingPoint, pastLeftHandGeometries, pastExistingPoints, pastAssignedPoints);

			// Step3: locate assigned point from existing point, angle and length
			var assignedPoint = LineSegment.LocateOtherEndPoint(existingPoint, assignedAngle, assignedLength);
			return assignedPoint;
		}

		internal static double AssignAngle(Point existingPoint, List<PolylineGeometry> pastLeftHandGeometries, List<Point> pastExistingPoints, List<Point> pastAssignedPoints)
		{
			// TODO: make a list of int of all connections to compile the "score" each connection get after going through the history
			for (int i = 0; i < pastLeftHandGeometries.Count; i++)
			{
				var plfg = pastLeftHandGeometries[i];
				var pep = pastExistingPoints[i];
				var pap = pastAssignedPoints[i];
				if (!ListUtilities.DoesContainItem(plfg.PolylinesCopy, pep))
				{
					throw new ArgumentException("the past existing point is not in the past left hand geometry at index: " + i.ToString());
				}

				var angle = pep.AngleTowardsPoint(pap);
				
				// Find 
			}
			return -1;
		}

		internal static double AssignLength(Point existingPoint, List<PolylineGeometry> pastLeftHandGeometries, List<Point> pastExistingPoints, List<Point> pastAssignedPoints)
		{
			for (int i = 0; i < pastLeftHandGeometries.Count; i++)
			{
				var plfg = pastLeftHandGeometries[i];
				var pep = pastExistingPoints[i];
				var pap = pastAssignedPoints[i];

				
			}

			return -1; // stub
		}
	}
}
