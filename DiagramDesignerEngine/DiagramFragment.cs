using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class DiagramFragment
	{
		private CycleOfLineSegments Perimeter;
		private List<LineSegment> SegmentsWithin; // these do not contain the perimeter

		internal DiagramFragment(CycleOfLineSegments perimeter, List<LineSegment> segmentsWithinPerimeter)
		{
			foreach (LineSegment ls in segmentsWithinPerimeter)
			{
				if (!perimeter.IsLineSegmentInCycle(ls))
				{
					throw new ArgumentException("not all segments are within the perimeter");
				}
			}
			this.Perimeter = perimeter;
			this.SegmentsWithin = segmentsWithinPerimeter;
		}

		/// <summary>
		/// Divide the fragment if possible, or return null (in which case this is a room) 
		/// </summary>
		/// <returns> The two divided fragments if dividable, or null if it's not </returns>
		internal Tuple<DiagramFragment, DiagramFragment> DivideIntoSmallerFragments()
		{
			// start with an end point shared by the perimeter and a segment within the perimeter
			foreach (LineSegment perimeterSegment in Perimeter.GetPerimeter())
			{
				// search for first point only to avoid duplicates
				var endPoint = perimeterSegment.FirstPoint;
				// search for connected segments in the pool
				var startSegments = new List<LineSegment>();
				foreach (LineSegment segment in SegmentsWithin)
				{
					if (segment.FirstPoint == endPoint || segment.SecondPoint == endPoint)
					{
						startSegments.Add(segment);
					}
				}
				
				// perform depth first search from this segment, until another another end point of the perimeter is reached 
				foreach(LineSegment startSegment in startSegments)
				{
					var pool = new List<LineSegment>(this.SegmentsWithin); // make a copy
					var path = new List<LineSegment>();

					var currentSegment = startSegment;
					LineSegment nextSegment;
					bool isFirstPointTheOneToSearch = !(startSegment.FirstPoint == endPoint); // search the second point if the first point is connected to the perimeter
					
					do
					{
						path.Add(currentSegment);

						// if this is connected to another point in perimeter, then a path is found
						var searchResult1 = isFirstPointTheOneToSearch ? 
							SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, Perimeter.GetPerimeter()) :
							SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, Perimeter.GetPerimeter());
						if (searchResult1.Count > 0)
						{
							// this is a path
							break;
						}

						// the path hasn't reach the other side yet. Find the next segment
						var searchResult2 = isFirstPointTheOneToSearch ?  
							SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, pool) :
							SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, pool);
						if (searchResult2.Count > 0)
						{
							nextSegment = searchResult2.Last(); // choose the one with largest angle (for no particular reason)
							var pointConnectedToNextSegment = isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint;
							isFirstPointTheOneToSearch = !(pointConnectedToNextSegment == nextSegment.FirstPoint); // if first point is connected then search the free second point

							currentSegment = nextSegment;
						}
						else
						{
							// this is a dangling segment. Remove it from the pool
							pool.Remove(currentSegment);
							path.Remove(currentSegment);
							if (path.Count == 0)
							{
								break; // the startSegment leads to nowhere; not finding any room from this handle
							}
							// roll back
							currentSegment = path.Last();
							path.RemoveAt(path.Count - 1);
						}
					} while()
										}
			}
		}

		// divide the fragment through the path
		// if cannot find such a path, pick another end point to start; if no end point is left, return null
	}

	private List<LineSegment> DepthFirstSearch
	}
}
