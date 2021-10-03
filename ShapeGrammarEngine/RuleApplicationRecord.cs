using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	internal readonly struct RuleApplicationRecord
	{
		public readonly PolylineGeometry GeometryBefore;
		public readonly PolylineGeometry GeometryAfter;
		public readonly Dictionary<int, Point> ReversedLabeling;

		public RuleApplicationRecord(PolylineGeometry geoBefore, PolylineGeometry geoAfter, Dictionary<Point, int> labeling)
		{
			if ((geoBefore is null) || (geoAfter is null) || (labeling is null))
			{
				throw new ArgumentNullException();
			}
			this.ReversedLabeling = GrammarRule.ReverseLabeling(labeling);
			this.GeometryBefore = geoBefore;
			this.GeometryAfter = geoAfter;
		}
	}
}
