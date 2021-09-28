using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	internal readonly struct RuleApplicationRecord
	{
		public readonly PolylineGeometry GeometryBefore;
		public readonly PolylineGeometry GeometryAfter;

		public RuleApplicationRecord(PolylineGeometry geoBefore, PolylineGeometry geoAfter)
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
