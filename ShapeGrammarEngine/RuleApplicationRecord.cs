using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	public readonly struct RuleApplicationRecord
	{
		public readonly PolylinesGeometry GeometryBefore;
		public readonly PolylinesGeometry GeometryAfter;
		public readonly LabelingDictionary Labeling;

		public RuleApplicationRecord(PolylinesGeometry geoBefore, PolylinesGeometry geoAfter, LabelingDictionary labeling)
		{
			if ((geoBefore is null) || (geoAfter is null) || (labeling is null))
			{
				throw new ArgumentNullException();
			}
			this.Labeling = labeling;
			this.GeometryBefore = geoBefore;
			this.GeometryAfter = geoAfter;
		}
	}
}
