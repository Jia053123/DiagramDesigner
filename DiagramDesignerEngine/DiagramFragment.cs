using System;
using System.Collections.Generic;
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
		/// all dangling segments must connect with the perimeter </param>
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
			if (SegmentsWithin.Count == 0)
			{
				return null;
			}

			return null; // stub

			// start with a dangling segment, which is guaranteed to connect with the perimeter

			
			
		}

		// divide the fragment through the path
		// if cannot find such a path, pick another end point to start; if no end point is left, return null
	}
}
