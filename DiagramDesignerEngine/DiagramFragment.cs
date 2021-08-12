using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class DiagramFragment
	{
		private CycleOfLineSegments Perimeter = null;
		private List<CycleOfLineSegments> InnerPerimeters = null;

		private List<LineSegment> SegmentsWithin = null; // these do not contain part of the perimeter

		/// <param name="perimeter"> the perimeter of this fragment </param>
		/// <param name="segmentsWithinPerimeter"> segments in the fragment that's not part of the perimeter; 
		/// all dangling segments must connect an endpoint on the perimeter </param>
		internal DiagramFragment(CycleOfLineSegments perimeter, List<LineSegment> segmentsWithinPerimeter)
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
				for (int j = i+1; j < segmentsToTest.Count; j++)
				{
					if (LineSegment.DoOverlap(segmentsToTest[i], segmentsToTest[j]))
					{
						throw new ArgumentException("segmentsWithinPerimeter contains a segment overlapping partially or fully with the perimeter");
					}
				}
			}

			this.Perimeter = perimeter;
			this.SegmentsWithin = segmentsWithinPerimeter;
			this.InnerPerimeters = new List<CycleOfLineSegments>(); // TODO: stub
		}

		internal CycleOfLineSegments GetPerimeter()
		{
			return new CycleOfLineSegments(this.Perimeter.GetPerimeter());
		}

		internal List<CycleOfLineSegments> GetInnerPerimeters()
		{
			var list = new List<CycleOfLineSegments>();
			foreach (CycleOfLineSegments cycle in this.InnerPerimeters)
			{
				list.Add(new CycleOfLineSegments(cycle.GetPerimeter()));
			}
			return list;
		}

		internal List<LineSegment> GetSegmentsWithin()
		{
			return new List<LineSegment>(this.SegmentsWithin);
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
					// divide the fragment through the path	
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

					var pathThroughPerimeter = this.ConvertPointsToPolyline(pathThroughPerimeterInPoints.GetRange(0, pathEndIndex+1));
					firstHalfOfPerimeter.AddRange(pathThroughPerimeter);
					secondHalfOfPerimeter.AddRange(pathThroughPerimeter);

					var subFragmentPerimeter1 = new CycleOfLineSegments(firstHalfOfPerimeter);
					var subFragmentPerimeter2 = new CycleOfLineSegments(secondHalfOfPerimeter);

					List<LineSegment> segmentsWithinSubFragment1 = new List<LineSegment>();
					foreach (LineSegment ls in this.SegmentsWithin)
					{
						if (subFragmentPerimeter1.IsLineSegmentInCycle(ls))
						{
							segmentsWithinSubFragment1.Add(ls);
						}
					}
					var subFragment1 = new DiagramFragment(subFragmentPerimeter1, segmentsWithinSubFragment1);

					List<LineSegment> segmentsWithinSubFragment2 = new List<LineSegment>();
					foreach (LineSegment ls in this.SegmentsWithin)
					{
						if (subFragmentPerimeter2.IsLineSegmentInCycle(ls))
						{
							segmentsWithinSubFragment2.Add(ls);
						}
					}
					var subFragment2 = new DiagramFragment(subFragmentPerimeter2, segmentsWithinSubFragment2);

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

		private bool IsAnEndpointOnPerimeter(Point point)
		{
			foreach (LineSegment ls in this.Perimeter.GetPerimeter())
			{
				if (ls.FirstPoint == point || ls.SecondPoint == point)
				{
					return true;
				}
			}
			return false;
		}

		private List<LineSegment> ConvertPointsToPolyline(List<Point> points)
		{
			List<LineSegment> polyline = new List<LineSegment>();
			for (int i = 1; i< points.Count; i++)
			{
				polyline.Add(new LineSegment(points[i - 1], points[i]));
			}
			return polyline;
		}

		private List<Point> ConvertPolyLineToEndpoints(List<LineSegment> polyline)
		{
			// TODO: should I check segments are connected?

			List<Point> endpoints = new List<Point>();

			// add the first point
			var firstSegment = polyline[0];
			var secondSegment = polyline[1];

			bool towardsFirstPoint;
			if (firstSegment.FirstPoint == secondSegment.FirstPoint || firstSegment.FirstPoint == secondSegment.SecondPoint)
			{
				towardsFirstPoint = true;
				endpoints.Add(firstSegment.SecondPoint);
			}
			else
			{
				towardsFirstPoint = false;
				endpoints.Add(firstSegment.FirstPoint);
			}

			for (int i = 0; i < polyline.Count; i++)
			{
				var segment = polyline[i];
				endpoints.Add(towardsFirstPoint ? segment.FirstPoint : segment.SecondPoint);
				if (i < polyline.Count -1)
				{
					// not at the end yet
					towardsFirstPoint = endpoints.Last() == polyline[i + 1].SecondPoint;
				}
			}

			return endpoints;
		}
	}

	

}
