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
			// find points to split for overlapping segments
			List<List<Point>> pointsToSplitForEachLine1 = new List<List<Point>>();
			for (int i = 0; i < SegmentsToExplode.Count; i++)
			{
				pointsToSplitForEachLine1.Add(new List<Point>());
			}

			for (int i = 0; i < this.SegmentsToExplode.Count; i++)
			{
				for (int j = i + 1; j < this.SegmentsToExplode.Count; j++)
				{
					// if segments overlap, remember where to split
					var pointsToSplit = LineSegment.PointsToSplitIfOverlap(SegmentsToExplode[i], SegmentsToExplode[j]);
					pointsToSplitForEachLine1[i].AddRange(pointsToSplit);
					pointsToSplitForEachLine1[j].AddRange(pointsToSplit);
				}
			}

			// split segments at points identified
			var collapsedSegments = new List<LineSegment>();
			for (int i = 0; i < pointsToSplitForEachLine1.Count; i++)
			{
				collapsedSegments.AddRange(collapsedSegments[i].SplitAtPoints(pointsToSplitForEachLine1[i]));
			}


			for (int i = 0; i < this.SegmentsToExplode.Count; i++)
			{
				for (int j = i + 1; j < this.SegmentsToExplode.Count; j++)
				{
					var result = LineSegment.SplitIfOverlap(this.SegmentsToExplode[i], this.SegmentsToExplode[j]);
					collapsedSegments.AddRange(result);
				}
			}
			// remove completely overlapping segments
			collapsedSegments = collapsedSegments.Distinct().ToList();

			// find intersections
			List<List<Point>> pointsToSplitForEachLine2 = new List<List<Point>>();
			for (int i = 0; i < collapsedSegments.Count; i++)
			{
				pointsToSplitForEachLine2.Add(new List<Point>());
			}
			for (int i = 0; i < collapsedSegments.Count; i++)
			{
				for (int j = i + 1; j < collapsedSegments.Count; j++)
				{
					// If intersection is found, remember which segment should be split at what point
					Point? pointToSplit = collapsedSegments[i].FindIntersection(collapsedSegments[j]);
					if (pointToSplit != null)
					{
						pointsToSplitForEachLine2[i].Add((Point)pointToSplit);
						pointsToSplitForEachLine2[j].Add((Point)pointToSplit);
					}
				}
			}

			// split segments at points identified
			var explodedWallSegments = new List<LineSegment>();
			for (int i = 0; i < pointsToSplitForEachLine2.Count; i++)
			{
				explodedWallSegments.AddRange(collapsedSegments[i].SplitAtPoints(pointsToSplitForEachLine2[i]));
			}

		
			return explodedWallSegments;
		}
	}
}
