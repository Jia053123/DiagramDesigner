using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	class GrammarRule
	{
		/// <summary>
		/// The shape on the left hand side of the rule. It shares labels with ShapeAfter
		/// </summary>
		public readonly Shape ShapeBefore;
		/// <summary>
		/// The shape on the right hand side of the rule. It shares labels with ShapeBefore  
		/// </summary>
		public readonly Shape ShapeAfter;

		/// <summary>
		/// shapeBefore and shapeAfter share the same set of labels, so having the same label in both parameters means 
		/// the point corresponding to that node will stay the same when the rule is applied
		/// </summary>
		public GrammarRule(Shape shapeBefore, Shape shapeAfter)
		{
			this.ShapeBefore = shapeBefore;
			this.ShapeAfter = shapeAfter;
		}
	}
}
