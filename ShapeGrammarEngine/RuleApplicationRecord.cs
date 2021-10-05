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
		public readonly LabelingDictionary Labeling;

		public RuleApplicationRecord(PolylineGeometry geoBefore, PolylineGeometry geoAfter, LabelingDictionary labeling)
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
