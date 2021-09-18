using System.Collections.Generic;
using BasicGeometries;
using DiagramDesignerEngine;
using ShapeGrammarEngine;

namespace DiagramDesignerModel
{
	public class PolylineGeometry
    {
        public List<Point> PathsDefinedByPoints { get; internal set; } = new List<Point>();
        internal Shape UnderlyingShape { get; private set; }

        internal List<LineSegment> ConvertToLineSegments()
		{
            List<LineSegment> list = new List<LineSegment>();
            for (int i = 0; i < this.PathsDefinedByPoints.Count-1; i++)
			{
                var lineSeg = new LineSegment(this.PathsDefinedByPoints[i], this.PathsDefinedByPoints[i + 1]);
                list.Add(lineSeg);
			}
            return list;
		}
	}
}
