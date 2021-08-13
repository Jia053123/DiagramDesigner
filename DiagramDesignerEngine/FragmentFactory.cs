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
		/// Extract fragments from line segments
		/// </summary>
		/// <param name="lineSegments"> The segments must be in exploded and non-overlapping state </param>
		/// <returns> the fragments able to be extracted from the segments </returns>
		static internal List<DiagramFragment> MakeFragments(List<LineSegment> lineSegments)
		{
			if (lineSegments is null || lineSegments.Count == 0)
			{
				throw new ArgumentException("lineSegments is null or empty");
			}

			// Find the segments with the left most starting point, which is guarantted to be a boundary point. 
			List<LineSegment> segmentsWithLeftmostFirstPoint = new List<LineSegment> { lineSegments[0] };
			for (int i = 1; i < lineSegments.Count; i++) 
			{
				if (lineSegments[i].FirstPoint.coordinateX < segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
				{
					// this one's first point is even more on the left
					segmentsWithLeftmostFirstPoint = new List<LineSegment> { lineSegments[i] };
				}
				else if (lineSegments[i].FirstPoint.coordinateX == segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
				{
					// this one's first point is also on the far left
					segmentsWithLeftmostFirstPoint.Add(lineSegments[i]);
				}
			}

			// from these segments find the one closest to a vertical line with the other endpoint at top
			Debug.Assert(segmentsWithLeftmostFirstPoint.Count > 0);
			var startPoint = segmentsWithLeftmostFirstPoint.First().FirstPoint;
			LineSegment testSegment = new LineSegment(new Point(startPoint.coordinateX, startPoint.coordinateY - 10), startPoint);
			double smallestAngleClockwise = TraversalUtilities.AngleAmongTwoSegments(testSegment, segmentsWithLeftmostFirstPoint[0]);
			LineSegment startSegment = segmentsWithLeftmostFirstPoint[0];
			for (int i = 1; i < segmentsWithLeftmostFirstPoint.Count; i++)
			{
				var angle = TraversalUtilities.AngleAmongTwoSegments(testSegment, ls);
				if (angle == 0)
				{
					angle = Math.PI * 2;
				}

			}



			// Initialize the starting handle
			var startPoint = segmentsWithLeftmostFirstPoint.First().FirstPoint;
			LineSegment startingHandle = new LineSegment(new Point(startPoint.coordinateX, startPoint.coordinateY - 10), startPoint);

			bool IsFirstPointTheOneToSearch = false; // for the first segment the first point is connected to the handel
			var currentSegment = startingHandle;

			// TODO: refactor to deal with dangling segments first! 
			// TODO: copy segments before removing

			// Find a perimeter by keeping turning the largest angle
			var perimeter = new List<LineSegment>();
			LineSegment nextSegment;
			do
			{
				perimeter.Add(currentSegment);

				// find the next segment
				if (IsFirstPointTheOneToSearch)
				{
					var result = SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, segments);
					if (result.Count > 0)
					{
						nextSegment = result.Last(); // choose the one with largest angle
						IsFirstPointTheOneToSearch = !(currentSegment.FirstPoint == nextSegment.FirstPoint);

						currentSegment = nextSegment;
					}
					else
					{
						// this is a dangling segment and cannot be part of any room. Remove it from the pool
						segments.Remove(currentSegment);
						perimeter.Remove(currentSegment);
						if (perimeter.Count == 0)
						{
							break; // the segments lead to nowhere; not finding any room from this handle
						}
						// roll back
						currentSegment = perimeter.Last();
						perimeter.RemoveAt(perimeter.Count - 1);
					}
				}
				else
				{
					var result = SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, segments);
					if (result.Count > 0)
					{
						nextSegment = result.Last(); // choose the one with largest angle
						IsFirstPointTheOneToSearch = !(currentSegment.SecondPoint == nextSegment.FirstPoint);

						currentSegment = nextSegment;
					}
					else
					{
						// this is a dangling segment and cannot be part of any room. Remove it
						segments.Remove(currentSegment);
						perimeter.Remove(currentSegment);
						if (perimeter.Count == 0)
						{
							break; // not finding any room from this handle
						}
						// roll back
						currentSegment = perimeter.Last();
						perimeter.RemoveAt(perimeter.Count - 1);
					}
				}
			} while (!perimeter.Contains(currentSegment));

			perimeter.RemoveAt(0); // remove the handle! 

			if (perimeter.Count > 2)
			{
				// found a perimeter! 
				var cycle = new CycleOfLineSegments(perimeter);

				// TODO: make a DaigramFragment from the perimeter and all included segments and push into a stack. Remove all these segments from FragmentsLeftToResolve
			}
		} while (this.FragmentsLeftToResolve.Count > 0);
		}

	}
}
