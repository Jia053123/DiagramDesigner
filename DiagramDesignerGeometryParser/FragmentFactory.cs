﻿using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DiagramDesignerGeometryParser
{
	public static class FragmentFactory
	{
		/// <summary>
		/// Make and keep dividing fragments until all fragments are undividable (effectively rooms). 
		/// The leftover segments cannot be part of a room. 
		/// </summary>
		/// <param name="explodedLineSegments"> The segments must be in exploded and non-overlapping state </param>
		public static List<UndividableDiagramFragment> ExtractAllFragments(List<LineSegment> explodedLineSegments)
		{
			var dividableFragments = FragmentFactory.MakeFragments(explodedLineSegments).Cast<DiagramFragment>().ToList();
			Stack<DiagramFragment> fragmentsToResolve = new Stack<DiagramFragment>(dividableFragments);
			List<UndividableDiagramFragment> extractedFragments = new List<UndividableDiagramFragment>();

			while (fragmentsToResolve.Count > 0)
			{
				var ftr = fragmentsToResolve.Pop();

				if (ftr is DividableDiagramFragment)
				{
					var dividedFtr = ((DividableDiagramFragment)ftr).DivideIntoSmallerFragments();
					foreach (DiagramFragment df in dividedFtr)
					{
						fragmentsToResolve.Push(df);
					}
				}
				else if (ftr is UndividableDiagramFragment)
				{
					extractedFragments.Add((UndividableDiagramFragment)ftr);
				}
			}
			return extractedFragments;
		}


		/// <summary>
		/// Extract fragments from line segments. It is done by only one layer so an extracted fragment does not contain another. 
		/// The leftover segments cannot be part of a room. 
		/// </summary>
		/// <param name="lineSegments"> The segments must be in exploded and non-overlapping state </param>
		/// <returns> the fragments able to be extracted from the segments. They do not contain each other </returns>
		static internal List<DividableDiagramFragment> MakeFragments(List<LineSegment> lineSegments)
		{
			if (lineSegments is null) { throw new ArgumentException("lineSegments is null"); }

			// Start by removing dangling line segments, which cannot be part of a room
			var trimmedSegments = TraversalUtilities.RemoveDanglingLineSegments(lineSegments);

			var fragmentsMade = new List<DividableDiagramFragment>();
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

				double smallestAngleClockwise = double.NaN; 
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
				var traverser = new LineSegmentsTraverser(trimmedSegments);
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
				var newFragment = new DividableDiagramFragment(newPerimeter, segmentsWithinPerimeter);
				fragmentsMade.Add(newFragment);

				// setup before next check
				trimmedSegments = trimmedSegmentsCopy;
				trimmedSegments = TraversalUtilities.RemoveDanglingLineSegments(trimmedSegments);
			}

			return fragmentsMade;
		}

	}
}
