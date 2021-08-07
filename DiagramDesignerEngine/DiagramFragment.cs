using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
	class DiagramFragment
	{
		private CycleOfLineSegments Perimeter;
		private List<LineSegment> LineSegments; // these do not contain the perimeter

		internal DiagramFragment(CycleOfLineSegments perimeter, List<LineSegment> segmentsWithinPerimeter)
		{
			foreach (LineSegment ls in segmentsWithinPerimeter)
			{
				
			}
			
		}

		/// <summary>
		/// Divide the fragment if possible, or return null if this is already a room (not dividable)
		/// </summary>
		/// <returns> The two divided fragments if dividable, or null if it's not </returns>
		internal Tuple<DiagramFragment, DiagramFragment> DivideIntoSmallerFragments()
		{
			
			
		}
	}
}
