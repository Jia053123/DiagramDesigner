using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class DividableDiagramFragment: DiagramFragment
	{
		/// <param name="perimeter"> the perimeter of this fragment </param>
		/// <param name="segmentsWithinPerimeter"> segments in the fragment that's not part of the perimeters. 
		/// All dangling segments must connect an endpoint on the perimeters; </param> 
		internal DividableDiagramFragment(CycleOfLineSegments perimeter, List<LineSegment> segmentsWithinPerimeter)
		{
			foreach (LineSegment ls in segmentsWithinPerimeter)
			{
				if (!perimeter.IsLineSegmentInCycle(ls))
				{
					throw new ArgumentException("not all segments are within the perimeter");
				}
			}

			var segmentsToTest = perimeter.GetPerimeter();
			segmentsToTest.AddRange(segmentsWithinPerimeter);
			var testedSegments = TraversalUtilities.RemoveDanglingLineSegments(segmentsToTest);
			if (testedSegments.Count < segmentsToTest.Count)
			{
				throw new ArgumentException("segmentsWithinPerimeter contains a dangling segment not connected to the perimeter");
			}
			for (int i = 0; i < segmentsToTest.Count; i++)
			{
				for (int j = i + 1; j < segmentsToTest.Count; j++)
				{
					if (LineSegment.DoOverlap(segmentsToTest[i], segmentsToTest[j]))
					{
						throw new ArgumentException("segmentsWithinPerimeter contains a segment overlapping partially or fully with the perimeter");
					}
				}
			}

			this.Perimeter = perimeter;
			this.SegmentsWithin = segmentsWithinPerimeter;
			//this.InnerPerimeters = innerPerimeters;
		}

		/// <summary>
		/// Divide the fragment into smaller ones if possible
		/// </summary>
		/// <returns> The divided fragments if dividable, or null if it's not (in which case this is a room) </returns>
		internal List<DiagramFragment> DivideIntoSmallerFragments()
		{
			if (SegmentsWithin.Count == 0) { return null; }

			// find a line connected to the perimeter
			List<LineSegment> startSegments = new List<LineSegment>();
			List<bool> searchAtFirstPointForSegments = new List<bool>();
			foreach (LineSegment ls in this.SegmentsWithin)
			{
				if (this.IsAnEndpointOnPerimeter(ls.FirstPoint))
				{
					startSegments.Add(ls);
					searchAtFirstPointForSegments.Add(false);
				}
				else if (this.IsAnEndpointOnPerimeter(ls.SecondPoint))
				{
					startSegments.Add(ls);
					searchAtFirstPointForSegments.Add(true);
				}
			}

			if (startSegments.Count != 0)
			{
				List<Point> pathThroughPerimeterInPoints = null;
				// try to find a path through
				for (int i = 0; i < startSegments.Count; i++)
				{
					var traverser = new LineSegmentsTraverser(this.SegmentsWithin);
					Tuple<int, List<LineSegment>> result;
					result = traverser.TraverseSegments(startSegments[i], searchAtFirstPointForSegments[i], true); // last parameter shouldn't matter

					if (result.Item1 == -1)
					{
						// since all deadends connect to the perimeter, this is a solution
						pathThroughPerimeterInPoints = traverser.GetLastPointsAlongPath();
						break;
					}
				}

				if (!(pathThroughPerimeterInPoints is null))
				{
					// divide the perimeter through the path	
					var perimeterInPoints = this.ConvertPolyLineToEndpoints(this.Perimeter.GetPerimeter());
					var firstIndexOnPeri = perimeterInPoints.FindIndex(p => p == pathThroughPerimeterInPoints.First());
					int pathEndIndex = -1;
					int secondIndexOnPeri = -1;
					for (int i = 1; i < pathThroughPerimeterInPoints.Count; i++)
					{
						if (perimeterInPoints.Contains(pathThroughPerimeterInPoints[i]))
						{
							pathEndIndex = i;
							secondIndexOnPeri = perimeterInPoints.FindIndex(p => p == pathThroughPerimeterInPoints[i]);
							break;
						}
					}
					Debug.Assert(pathEndIndex != -1);
					Debug.Assert(secondIndexOnPeri != -1);

					var firstHalfOfPerimeter = this.ConvertPointsToPolyline(perimeterInPoints.GetRange(firstIndexOnPeri, secondIndexOnPeri - firstIndexOnPeri + 1));
					List<LineSegment> secondHalfOfPerimeter = this.Perimeter.GetPerimeter();
					foreach (LineSegment ls in firstHalfOfPerimeter)
					{
						var r = secondHalfOfPerimeter.Remove(ls);
						Debug.Assert(r);
					}

					var pathThroughPerimeter = this.ConvertPointsToPolyline(pathThroughPerimeterInPoints.GetRange(0, pathEndIndex + 1));
					firstHalfOfPerimeter.AddRange(pathThroughPerimeter);
					secondHalfOfPerimeter.AddRange(pathThroughPerimeter);

					var subFragmentPerimeter1 = new CycleOfLineSegments(firstHalfOfPerimeter);
					var subFragmentPerimeter2 = new CycleOfLineSegments(secondHalfOfPerimeter);

					// put other segments into respective halves
					List<LineSegment> segmentsWithinSubFragment1 = new List<LineSegment>();
					List<LineSegment> segmentsWithinSubFragment2 = new List<LineSegment>();
					foreach (LineSegment ls in this.SegmentsWithin)
					{
						if (subFragmentPerimeter1.IsLineSegmentInCycle(ls))
						{
							segmentsWithinSubFragment1.Add(ls);
						}
						else if (subFragmentPerimeter2.IsLineSegmentInCycle(ls))
						{
							segmentsWithinSubFragment2.Add(ls);
						}
					}
					// TODO: stub
					var subFragment1 = new DiagramFragment(subFragmentPerimeter1, new List<CycleOfLineSegments>(), segmentsWithinSubFragment1);
					var subFragment2 = new DiagramFragment(subFragmentPerimeter2, new List<CycleOfLineSegments>(), segmentsWithinSubFragment2);

					return new List<DiagramFragment> { subFragment1, subFragment2 };
				}
				else
				{
					// but there are still segments in there! Make them into new fragments
				}
			}
			else
			{
				// but there are still segments in there! Make them into new fragments

			}

			return null; // stub
		}
	}
}
