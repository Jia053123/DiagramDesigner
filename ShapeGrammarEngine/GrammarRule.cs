using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine
{
	class GrammarRule
	{
		/// <summary>
		/// The shape on the left hand side of the rule. It shares labels with ShapeAfter
		/// </summary>
		public Shape ShapeBefore { get; private set; }
		/// <summary>
		/// The shape on the right hand side of the rule. It shares labels with ShapeBefore  
		/// </summary>
		public Shape ShapeAfter { get; private set; }

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
		/// shapeBefore and shapeAfter share the same set of labels, so having the same label in both parameters means 
		/// the point corresponding to that node will stay the same when the rule is applied
		/// </summary>
		public GrammarRule(Shape shapeBefore, Shape shapeAfter)
		{
			this.ShapeBefore = shapeBefore;
			this.ShapeAfter = shapeAfter;
		}

		/// <summary>
		/// Learn how the rule can be applied from examples in terms of proportions. 
		/// </summary>
		/// <param name="geometryBefore"> The geometry in the example before the rule is applied </param>
		/// <param name="geometryAfter"> The geometry in the example after the rule is applied </param>
		public void LearnFromExample(PolylineGroup geometryBefore, PolylineGroup geometryAfter)
		{
			if (!this.ShapeBefore.ConformsWithGeometry(geometryBefore, out _))
			{
				throw new ArgumentException("geometryBefore does not conform with ShapeBefore");
			}
			if (!this.ShapeAfter.ConformsWithGeometry(geometryAfter, out _))
			{
				throw new ArgumentException("geometryAfter does not conform with ShapeAfter");
			}

			this.ApplicationRecords.Add(new RuleApplicationRecord(geometryBefore, geometryAfter));
		}

		/// <summary>
		/// Apply the rule with knowledge learnt from examples. 
		/// </summary>
		/// <param name="polylines"> the geometry on which the rule will be applied. It must confrom with ShapeBefore </param>
		/// <returns> the geometry after the rule is applied. It will confrom with ShapeAfter </returns>
		public PolylineGroup ApplyToGeometry(PolylineGroup polylines)
		{
			// step1: confirm that the input conforms with LHS (left hand shape) 
			if (!this.ShapeBefore.ConformsWithGeometry(polylines, out _))
			{
				throw new ArgumentException("polylines does not conform with ShapeBefore");
			}
			
			// step2: label the input



			return null; // stub
		}
	}
}
