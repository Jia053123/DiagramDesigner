using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	class Rule
	{
		public readonly Shape ShapeBefore;
		public readonly Shape ShapeAfter;

		/// <param name="shapeBefore"> The shape on the left hand side of the rule. It shares labels with shapeAfter </param>
		/// <param name="shapeAfter"> The shape on the right hand side of the rule. It shares labels with shapeBefore </param>
		public Rule(Shape shapeBefore, Shape shapeAfter)
		{

		}
	}
}
