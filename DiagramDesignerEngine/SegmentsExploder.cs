using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class SegmentsExploder
	{
		private List<LineSegment> SegmentsToExplode;

		internal SegmentsExploder(List<LineSegment> segmentsToExplode)
		{
			this.SegmentsToExplode = segmentsToExplode;
		}

		internal List<LineSegment> ExplodeSegments()
		{
			// split overlapping segments
			var collapsedSegments = new List<LineSegment>();
			for (int i = 0; i < this.SegmentsToExplode.Count; i++)
			{
				for (int j = i + 1; j < this.SegmentsToExplode.Count; j++)
				{
					var result = LineSegment.SplitIfOverlap(this.SegmentsToExplode[i], this.SegmentsToExplode[j]);
					collapsedSegments.AddRange(result);
				}
			}

			// find intersections
			List<List<Point>> pointsToSplitForEachLine = new List<List<Point>>();
			for (int i = 0; i < collapsedSegments.Count; i++)
			{
				pointsToSplitForEachLine.Add(new List<Point>());
			}
			for (int i = 0; i < collapsedSegments.Count; i++)
			{
				for (int j = i + 1; j < collapsedSegments.Count; j++)
				{
					// If intersection is found, remember which segment should be split at what point
					Point? pointToSplit = collapsedSegments[i].FindIntersection(collapsedSegments[j]);
					if (pointToSplit != null)
					{
						pointsToSplitForEachLine[i].Add((Point)pointToSplit);
						pointsToSplitForEachLine[j].Add((Point)pointToSplit);
					}
				}
			}

			// split segments at points identified
			var explodedWallSegments = new List<LineSegment>();
			for (int i = 0; i < pointsToSplitForEachLine.Count; i++)
			{
				explodedWallSegments.AddRange(collapsedSegments[i].SplitAtPoints(pointsToSplitForEachLine[i]));
			}

			//explodedWallSegments = explodedWallSegments.Distinct().ToList();

			return explodedWallSegments;
		}
	}
}
