using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	internal readonly struct RuleApplicationRecord
	{
		public readonly PolylineGroup GeometryBefore;
		public readonly PolylineGroup GeometryAfter;

		public RuleApplicationRecord(PolylineGroup geoBefore, PolylineGroup geoAfter)
		{
			if ((geoBefore is null) || (geoAfter is null))
			{
				throw new ArgumentNullException();
			}

			this.GeometryBefore = geoBefore;
			this.GeometryAfter = geoAfter;
		}
	}
}
