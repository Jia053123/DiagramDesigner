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

		private List<LineSegment> SegmentsWithin = null;

		/// <param name="perimeter"> the perimeter of this fragment </param>
		/// <param name="innerPerimeters"> the inner perimeters. For this to not be empty, segmentsWithinPerimeter must be empty.
		/// They must be inside the perimeter with no overlap, and they must not interset or overlap with each other </param>
		/// <param name="segmentsWithinPerimeter"> segments in the fragment that's not part of the perimeters. 
		/// For this to not be empty, innerPerimeters must be empty. All dangling segments must connect an endpoint on the perimeters; </param> 
		internal DiagramFragment(CycleOfLineSegments perimeter, List<CycleOfLineSegments> innerPerimeters, List<LineSegment> segmentsWithinPerimeter)
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

			// inner perimeters check against the perimeter
			foreach (CycleOfLineSegments ip in innerPerimeters)
			{
				foreach (LineSegment l in ip.GetPerimeter())
				{
					// check each segment of the inner perimeters
					if (! perimeter.IsLineSegmentInCycle(l))
					{
						throw new ArgumentException("an inner perimeter segment is outside the perimeter");
					}
					foreach (LineSegment pl in perimeter.GetPerimeter())
					{
						if (LineSegment.DoOverlap(pl, l))
						{
							throw new ArgumentException("an inner perimeter segment overlap with part of the perimeter");
						}
					}
				}
			}
			// inner perimeters check against each other
			for (int i = 0; i < innerPerimeters.Count; i++)
			{
				for (int j = i+1; j < innerPerimeters.Count; j++)
				{
					var innerPeri1 = innerPerimeters[i];
					var innerPeri1List = innerPeri1.GetPerimeter();
					var innerPeri2 = innerPerimeters[j];
					var innerPeri2List = innerPeri2.GetPerimeter();

					foreach (LineSegment ls in innerPeri2List)
					{
						if (innerPeri1.IsLineSegmentInCycle(ls))
						{
							throw new ArgumentException("an inner perimeter segment is in another inner perimeter");
						}
					}
					foreach (LineSegment ls in innerPeri1List)
					{
						if (innerPeri2.IsLineSegmentInCycle(ls))
						{
							throw new ArgumentException("an inner perimeter segment is in another inner perimeter");
						}
					}

					for (int a = 0; a < innerPeri1List.Count; a++)
					{
						for (int b = 0; b < innerPeri2List.Count; b++)
						{
							var ls1 = innerPeri1List[a];
							var ls2 = innerPeri2List[b];
							if (LineSegment.DoOverlap(ls1, ls2))
							{
								throw new ArgumentException("two inner perimeter segments overlap");
							}
						}
					}
				}
			}

			this.Perimeter = perimeter;
			this.SegmentsWithin = segmentsWithinPerimeter;
			this.InnerPerimeters = innerPerimeters;
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

					var pathThroughPerimeter = this.ConvertPointsToPolyline(pathThroughPerimeterInPoints.GetRange(0, pathEndIndex+1));
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

		internal bool IsLineSegmentInPerimeters(LineSegment ls)
		{
			// within the outer perimeter
			if (this.Perimeter.IsLineSegmentInCycle(ls))
			{
				// outside each inner perimeter
				foreach (CycleOfLineSegments innerPerimeter in this.InnerPerimeters)
				{
					if (innerPerimeter.IsLineSegmentInCycle(ls))
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				return false;
			}
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
