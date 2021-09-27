using BasicGeometries;
using System;
using System.Collections.Generic;

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

		private List<RuleApplicationRecord> ApplicationRecords = new List<RuleApplicationRecord>();

		/// <summary>
		/// Create a shape grammar rule given its both sides
		/// leftHandShape and rightHandShape share the same set of labels, so having the same label in both parameters means 
		/// the point corresponding to that node will stay the same when the rule is applied
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
		public static GrammarRule CreateGrammarRuleFromOneExample(PolylineGroup geometryBefore, PolylineGroup geometryAfter, out Dictionary<Point, int> labeling)
		{
			Dictionary<Point, int> lhsLabeling, sharedLabeling;
			var lhs = Shape.CreateShapeFromPolylines(geometryBefore, null, out lhsLabeling);
			var rhs = Shape.CreateShapeFromPolylines(geometryAfter, lhsLabeling, out sharedLabeling);

			labeling = sharedLabeling;
			return new GrammarRule(lhs, rhs);
		}

		/// <summary>
		/// Learn how the rule can be applied from examples in terms of proportions. 
		/// </summary>
		/// <param name="geometryBefore"> The geometry in the example before the rule is applied </param>
		/// <param name="geometryAfter"> The geometry in the example after the rule is applied </param>
		public void LearnFromExample(PolylineGroup geometryBefore, PolylineGroup geometryAfter)
		{
			if (!this.LeftHandShape.ConformsWithGeometry(geometryBefore, out _))
			{
				throw new ArgumentException("geometryBefore does not conform with ShapeBefore");
			}
			if (!this.RightHandShape.ConformsWithGeometry(geometryAfter, out _))
			{
				throw new ArgumentException("geometryAfter does not conform with ShapeAfter");
			}

			this.ApplicationRecords.Add(new RuleApplicationRecord(geometryBefore, geometryAfter));
		}

		/// <summary>
		/// Apply the rule with knowledge learnt from examples. 
		/// </summary>
		/// <param name="polylines"> the geometry on which the rule will be applied. It must confrom with LeftHandShape </param>
		/// <returns> the geometry after the rule is applied. It will confrom with RightHandShape </returns>
		public PolylineGroup ApplyToGeometry(PolylineGroup polylines)
		{
			// Step1: check and label the polylines
			Dictionary<Point, int> labeling;
			var doesConform = this.LeftHandShape.ConformsWithGeometry(polylines, out labeling);
			if (! doesConform)
			{
				throw new ArgumentException("polylines does not conform with ShapeBefore");
			}
			
			// Step2: remove the points to be removed
			

			return null; // stub
		}
	}
}
