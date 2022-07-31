using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DiagramDesignerGeometryParser
{
	class DividableDiagramFragment : DiagramFragment
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
		}

		/// <summary>
		/// Divide the fragment into smaller ones if possible
		/// </summary>
		/// <returns> The divided fragments if dividable, or one UndividableFragment if it's not </returns>
		internal List<DiagramFragment> DivideIntoSmallerFragments()
		{
			if (SegmentsWithin.Count == 0)
			{
				return new List<DiagramFragment> { new UndividableDiagramFragment(this.Perimeter, new List<CycleOfLineSegments>()) };
			}

			var pathThroughPerimeterInPoints = this.FindPathThroughPerimeter();
			if (!(pathThroughPerimeterInPoints is null))
			{
				var result = this.DivideAlongPath(pathThroughPerimeterInPoints);
				return new List<DiagramFragment> { result.Item1, result.Item2 };
			}

			// couldn't find a dividing path, but there are still segments in there! Make them into new fragments
			var results = FragmentFactory.MakeFragments(this.SegmentsWithin);
			var innerPerimeters = new List<CycleOfLineSegments>();
			foreach (DividableDiagramFragment df in results)
			{
				List<LineSegment> p = df.GetPerimeter().GetPerimeter();
				innerPerimeters.Add(new CycleOfLineSegments(p));
			}

			var largeFragment = new UndividableDiagramFragment(this.Perimeter, innerPerimeters);

			var fragmentsMade = new List<DiagramFragment>();
			fragmentsMade.Add(largeFragment);
			fragmentsMade.AddRange(results);
			return fragmentsMade;
		}

		/// <summary>
		/// Find a path that connects to two distinct end points on the perimeter, capable of dividing the polygon into two parts
		/// </summary>
		/// <returns> the points, in order, along the path through; or null if cannot find one </returns>
		private List<Point> FindPathThroughPerimeter()
		{
			// find all the lines (startSegments) connected to the perimeter 
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
				// try to find a path through from each startSegment
				for (int i = 0; i < startSegments.Count; i++)
				{
					var traverser = new LineSegmentsTraverser(this.SegmentsWithin);
					Tuple<int, List<LineSegment>> traversalResult;
					traversalResult = traverser.TraverseSegments(startSegments[i], searchAtFirstPointForSegments[i], true); // last parameter shouldn't matter

					while (!(traversalResult is null))
					{
						// look for the closest second point on the perimeter
						var pointsAlongPath = traverser.GetLastPointsAlongPath();
						for (int j = 1; j < pointsAlongPath.Count; j++)
						{
							if (this.IsAnEndpointOnPerimeter(pointsAlongPath[j]) && pointsAlongPath[j] != pointsAlongPath[0])
							{
								return traverser.GetLastPointsAlongPath().GetRange(0, j + 1);
							}
						}
						// couldn't find a path through... try again from the same startSegment
						traversalResult = traverser.TraverseAgain();
					}
				}
			}

			// no startSegment found or none of them works
			return null;
		}

		/// <summary>
		/// Divide the fragment into two halves along the given path
		/// </summary>
		/// <param name="pointsAlongPath"> the list of points along the dividing path </param>
		private (DividableDiagramFragment, DividableDiagramFragment) DivideAlongPath(List<Point> pointsAlongPath)
		{
			if (pointsAlongPath is null) 
			{ 
				throw new ArgumentException("pointsAlongPath is null"); 
			}

			// find the two dividing points on the perimeter
			var perimeterInPoints = this.ConvertPolyLineToEndpoints(this.Perimeter.GetPerimeter());
			int oneIndexOnPeri = perimeterInPoints.FindIndex(p => p == pointsAlongPath.First());
			int anotherIndexOnPeri = perimeterInPoints.FindIndex(p => p == pointsAlongPath.Last());

			if (oneIndexOnPeri == -1 || anotherIndexOnPeri == -1) 
			{ 
				throw new ArgumentException("pointsAlongPath does not divide the fragment"); 
			}

			// divide the perimeter into two halves at those two points
			var smallerIndex = oneIndexOnPeri < anotherIndexOnPeri ? oneIndexOnPeri : anotherIndexOnPeri;
			var firstHalfOfPerimeterCount = oneIndexOnPeri < anotherIndexOnPeri ? (anotherIndexOnPeri - oneIndexOnPeri + 1) : (oneIndexOnPeri - anotherIndexOnPeri + 1);
			var firstHalfOfPerimeter = this.ConvertPointsToPolyline(perimeterInPoints.GetRange(smallerIndex, firstHalfOfPerimeterCount));

			List<LineSegment> secondHalfOfPerimeter = this.Perimeter.GetPerimeter();
			foreach (LineSegment ls in firstHalfOfPerimeter)
			{
				var r = secondHalfOfPerimeter.Remove(ls);
				Debug.Assert(r);
			}

			// add the dividing path to both halves to from two perimeters
			var pathThroughPerimeter = this.ConvertPointsToPolyline(pointsAlongPath);
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
			var subFragment1 = new DividableDiagramFragment(subFragmentPerimeter1, segmentsWithinSubFragment1);
			var subFragment2 = new DividableDiagramFragment(subFragmentPerimeter2, segmentsWithinSubFragment2);

			return (subFragment1, subFragment2);
		}
	}
}
