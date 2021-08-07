using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
	class LineSegmentsTraverser
	{
		private List<LineSegment> SegmentsToTraverse;
		private List<LineSegment> Path;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="segmentsToTraverse"> The pool of segments to traverse without duplicate segments </param>
		internal LineSegmentsTraverser(List<LineSegment> segmentsToTraverse)
		{
			this.SegmentsToTraverse = segmentsToTraverse;
		}

		/// <summary>
		/// Tranverse a pool of segments until the end of the path connects back to itself or reaches a deadend
		/// </summary>
		/// <param name="startSegment"> The segment from segmentsToTraverse. The beginning of the path. </param>
		/// <param name="startWithFirstPoint"> start traversing from FirstPoint of the startSegment or not (and use SecondPoint) </param>
		/// <param name="turnLargestAngleFirst"> when the potential path branches, turn the largest angle or not (and turn the smallest angle) </param>
		/// <returns> The path traversed </returns>
		internal List<LineSegment> TraverseFromSegment(LineSegment startSegment, bool startWithFirstPoint, bool turnLargestAngle)
		{
			
		}
	}
}
