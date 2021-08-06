using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class RoomsFinder
	{
		private List<LineSegment> LinkedLineSegments;
		private Stack<List<LineSegment>> FragmentsLeftToResolve;

		internal RoomsFinder(List<LineSegment> explodedSegments)
		{
			this.LinkedLineSegments = new List<LineSegment>();

			// init resolution stack with exploded segments
			this.FragmentsLeftToResolve = new Stack<List<LineSegment>>();
			this.FragmentsLeftToResolve.Push(this.LinkedLineSegments);
		}

		internal List<EnclosedProgram> FindRooms()
		{
			do
			{
				var segments = this.FragmentsLeftToResolve.Pop();
				// Find the segment with the left most starting point, which is guarantted to be a boundary point. 
				List<LineSegment> segmentsWithLeftmostFirstPoint = new List<LineSegment> { segments[0] };
				for (int i = 1; i< segments.Count; i++)
				{
					if(segments[i].FirstPoint.coordinateX < segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
					{
						// this one's first point is even more on the left
						segmentsWithLeftmostFirstPoint = new List<LineSegment> { segments[i] };
					} 
					else if (segments[i].FirstPoint.coordinateX == segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
					{
						// this one's first point is also on the far left
						segmentsWithLeftmostFirstPoint.Add(segments[i]);
					}
				}

				// Initialize the starting handle and find the starting segment with the handle
				var startPoint = segmentsWithLeftmostFirstPoint.First().FirstPoint;
				LineSegment startingHandle = new LineSegment(new Point(startPoint.coordinateX, startPoint.coordinateY + 10), startPoint);
				

				// Find a perimeter by keeping turning the smallest angle
				
				

			} while (this.FragmentsLeftToResolve.Count > 0);


		}



		/// <summary>
		/// Find the indexes of segments connected to the FirstPoint of the segment from the pool, sorted by their angle from the segment
		/// </summary>
		/// <param name="segmentIndex"> the beginning segment </param>
		/// <param name="segmentsPool"> the list of segments to search from </param>
		/// <returns> the list of segments from the pool connected to the FirstPoint of the segment, 
		/// excluding segments equal to the input segment; 
		/// the list is sorted in ascending order by their angle from the beginning segemnt measured clockwise. </returns>
		private List<LineSegment> FindPreviousSegmentsSortedByAngle(LineSegment segment, List<LineSegment> segmentsPool)
		{
			var connectedSegments = new List<LineSegment>();
			for (int i = 0; i< segmentsPool.Count; i++)
			{ 
				if (segmentsPool[i] != segment)
				{
					if (segment.FirstPoint == segmentsPool[i].FirstPoint || segment.FirstPoint == segmentsPool[i].SecondPoint)
					{
						connectedSegments.Add(segmentsPool[i]);
					}
				}
			}
			return GeometryUtilities.SortSegmentsByAngleFromSegment(segment, connectedSegments);
		}

		/// <summary>
		/// Find the indexes of segments connected to the SecondPoint of the segment from the pool, sorted by their angle from the segment
		/// </summary>
		/// <param name="segmentIndex"> the index of the segment from the pool </param>
		/// <param name="pool"> the list of segments to search from </param>
		/// <returns> the list of indexes of segments connected to the SecondPoint of the input segment; 
		/// the list is sorted in ascending order by their angle from the segemnt measured in radian, clockwise. </returns>
		private List<LineSegment> FindNextSegmentsIndexSortedByAngle(LineSegment segment, List<LineSegment> segmentsPool)
		{
			var connectedSegments = new List<LineSegment>();
			for (int i = 0; i < segmentsPool.Count; i++)
			{
				if (segmentsPool[i] != segment)
				{
					if (segment.SecondPoint == segmentsPool[i].FirstPoint || segment.SecondPoint == segmentsPool[i].SecondPoint)
					{
						connectedSegments.Add(segmentsPool[i]);
					}
				}
			}
			return GeometryUtilities.SortSegmentsByAngleFromSegment(segment, connectedSegments);
		}
	}
}
