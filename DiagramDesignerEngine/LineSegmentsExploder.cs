﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class LineSegmentsExploder
	{
		private List<LineSegment> SegmentsToExplode;

		internal LineSegmentsExploder(List<LineSegment> segmentsToExplode)
		{
			this.SegmentsToExplode = segmentsToExplode;
		}

		internal List<LineSegment> ExplodeSegments()
		{
			var collapsedSegments = this.SplitAndMergeOverlappingSegments(SegmentsToExplode);
			List<LineSegment> explodedWallSegments = this.SplitIntersectingSegments(collapsedSegments);

			return explodedWallSegments;
		}

		private List<LineSegment> SplitAndMergeOverlappingSegments(List<LineSegment> segments)
		{
			List<List<Point>> pointsToSplitForEachLine1 = new List<List<Point>>();
			for (int i = 0; i < segments.Count; i++)
			{
				pointsToSplitForEachLine1.Add(new List<Point>());
			}

			// find points to split for overlapping segments
			for (int i = 0; i < segments.Count; i++)
			{
				for (int j = i + 1; j < segments.Count; j++)
				{
					// if segments overlap, remember where to split
					var pointsToSplit = LineSegment.PointsToSplitIfOverlap(segments[i], segments[j]);
					pointsToSplitForEachLine1[i].AddRange(pointsToSplit);
					pointsToSplitForEachLine1[j].AddRange(pointsToSplit);
				}
			}

			// split segments at points identified
			var collapsedSegments = new List<LineSegment>();
			for (int i = 0; i < pointsToSplitForEachLine1.Count; i++)
			{
				collapsedSegments.AddRange(segments[i].SplitAtPoints(pointsToSplitForEachLine1[i]));
			}
			// remove duplicate segments
			collapsedSegments = collapsedSegments.Distinct().ToList();

			return collapsedSegments;
		}

		private List<LineSegment> SplitIntersectingSegments(List<LineSegment> segments)
		{
			// find intersections
			List<List<Point>> pointsToSplitForEachLine2 = new List<List<Point>>();
			for (int i = 0; i < segments.Count; i++)
			{
				pointsToSplitForEachLine2.Add(new List<Point>());
			}

			for (int i = 0; i < segments.Count; i++)
			{
				for (int j = i + 1; j < segments.Count; j++)
				{
					// If intersection is found, remember which segment should be split at what point
					Point? pointToSplit = segments[i].FindIntersection(segments[j]);
					if (pointToSplit != null)
					{
						pointsToSplitForEachLine2[i].Add((Point)pointToSplit);
						pointsToSplitForEachLine2[j].Add((Point)pointToSplit);
					}
				}
			}

			// split segments at points identified
			var splitSegments = new List<LineSegment>();
			for (int i = 0; i < pointsToSplitForEachLine2.Count; i++)
			{
				splitSegments.AddRange(segments[i].SplitAtPoints(pointsToSplitForEachLine2[i]));
			}

			return splitSegments;
		}
	}
}
