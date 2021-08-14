using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	static class FragmentFactory
	{
		/// <summary>
		/// Extract fragments from line segments. The leftover segments cannot be part of a room
		/// </summary>
		/// <param name="lineSegments"> The segments must be in exploded and non-overlapping state </param>
		/// <returns> the fragments able to be extracted from the segments. </returns>
		static internal List<DiagramFragment> MakeFragments(List<LineSegment> lineSegments)
		{
			if (lineSegments is null) { throw new ArgumentException("lineSegments is null"); }

			// Start by removing dangling line segments, which cannot be part of a room
			var trimmedSegments = TraversalUtilities.RemoveDanglingLineSegments(lineSegments);

			var fragmentsMade = new List<DiagramFragment>();
			while (trimmedSegments.Count > 0)
			{
				// Find the segments with the left most starting point, which is guarantted to be a boundary point. 
				List<LineSegment> segmentsWithLeftmostFirstPoint = new List<LineSegment> { trimmedSegments[0] };
				for (int i = 1; i < trimmedSegments.Count; i++)
				{
					if (trimmedSegments[i].FirstPoint.coordinateX < segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
					{
						// this one's first point is even more on the left
						segmentsWithLeftmostFirstPoint = new List<LineSegment> { trimmedSegments[i] };
					}
					else if (trimmedSegments[i].FirstPoint.coordinateX == segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
					{
						// this one's first point is also on the far left
						segmentsWithLeftmostFirstPoint.Add(trimmedSegments[i]);
					}
				}

				// from these segments find the one closest to a vertical line with SecondPoint at top
				Debug.Assert(segmentsWithLeftmostFirstPoint.Count > 0);

				double smallestAngleClockwise = double.NaN; //TraversalUtilities.AngleAmongTwoSegments(testSegment, segmentsWithLeftmostFirstPoint[0]);
				LineSegment startSegment = segmentsWithLeftmostFirstPoint.First();
				for (int i = 0; i < segmentsWithLeftmostFirstPoint.Count; i++)
				{
					var startPoint = segmentsWithLeftmostFirstPoint[i].FirstPoint;
					LineSegment testSegment = new LineSegment(new Point(startPoint.coordinateX, startPoint.coordinateY - 10), startPoint);

					var angle = TraversalUtilities.AngleAmongTwoSegments(testSegment, segmentsWithLeftmostFirstPoint[i]);

					if (Double.IsNaN(smallestAngleClockwise) || angle < smallestAngleClockwise)
					{
						smallestAngleClockwise = angle;
						startSegment = segmentsWithLeftmostFirstPoint[i];
					}
				}

				// traverse starting from startSegemnt, at its SecondPoint, to find a perimeter
				var traverser = new LineSegmentsTraverser(lineSegments);
				var result = traverser.TraverseSegments(startSegment, false, false); // always turn the smallest angle possible to trace the perimeter

				Debug.Assert(result.Item1 != -1); // impossible to get stuck because there's no dangling segment
				var path = traverser.GetLastPath();
				var newPerimeter = new CycleOfLineSegments(path.GetRange(result.Item1, path.Count - result.Item1));

				// remove the perimeter from trimmedSegments
				foreach (LineSegment ls in newPerimeter.GetPerimeter())
				{
					var r = trimmedSegments.Remove(ls);
					Debug.Assert(r);
				}

				// make fragment with the found perimeter and everything within it. Remove them from trimmedSegments
				var trimmedSegmentsCopy = new List<LineSegment>(trimmedSegments); // copy in order to remove within foreach loop
				var segmentsWithinPerimeter = new List<LineSegment>();
				foreach (LineSegment ls in trimmedSegments)
				{
					if (newPerimeter.IsLineSegmentInCycle(ls))
					{
						var r = trimmedSegmentsCopy.Remove(ls);
						Debug.Assert(r);
						segmentsWithinPerimeter.Add(ls);
					}
				}
				var newFragment = new DiagramFragment(newPerimeter, new List<CycleOfLineSegments>(), segmentsWithinPerimeter);
				fragmentsMade.Add(newFragment);

				// setup before next check
				trimmedSegments = trimmedSegmentsCopy;
				trimmedSegments = TraversalUtilities.RemoveDanglingLineSegments(trimmedSegments);
			}

			return fragmentsMade;
		}

	}
}
