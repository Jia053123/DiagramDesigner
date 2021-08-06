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

			//// convert all segments to linked segments objects
			//foreach (LineSegment ls in explodedSegments)
			//{
			//	this.LinkedLineSegments.Add(new LineSegment(ls));	
			//}

			//// fill the previous and next segments lists of all linked segments
			//for (int i = 0; i < this.LinkedLineSegments.Count; i++)
			//{
			//	for (int j = i + 1; j < this.LinkedLineSegments.Count; j++)
			//	{
			//		if (this.LinkedLineSegments[i].WrappedLineSegment.FirstPoint == this.LinkedLineSegments[j].WrappedLineSegment.FirstPoint) 
			//		{
			//			this.LinkedLineSegments[i].PreviousSegments.Add(this.LinkedLineSegments[j]);
			//			this.LinkedLineSegments[j].PreviousSegments.Add(this.LinkedLineSegments[i]);
			//		}

			//		if (this.LinkedLineSegments[i].WrappedLineSegment.FirstPoint == this.LinkedLineSegments[j].WrappedLineSegment.SecondPoint)
			//		{
			//			this.LinkedLineSegments[i].PreviousSegments.Add(this.LinkedLineSegments[j]);
			//			this.LinkedLineSegments[j].NextSegments.Add(this.LinkedLineSegments[i]);
			//		}

			//		if (this.LinkedLineSegments[i].WrappedLineSegment.SecondPoint == this.LinkedLineSegments[j].WrappedLineSegment.FirstPoint)
			//		{
			//			this.LinkedLineSegments[i].NextSegments.Add(this.LinkedLineSegments[j]);
			//			this.LinkedLineSegments[j].PreviousSegments.Add(this.LinkedLineSegments[i]);
			//		}
			//		if (this.LinkedLineSegments[i].WrappedLineSegment.SecondPoint == this.LinkedLineSegments[j].WrappedLineSegment.SecondPoint)
			//		{
			//			this.LinkedLineSegments[i].NextSegments.Add(this.LinkedLineSegments[j]);
			//			this.LinkedLineSegments[j].NextSegments.Add(this.LinkedLineSegments[i]);
			//		}
			//	}
			//}

			//// sort?? 


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
		/// <param name="segmentIndex"> the index of the segment from the pool </param>
		/// <param name="pool"> the list of segments to search from </param>
		/// <returns> the list of indexes of segments connected to the FirstPoint of the input segment; 
		/// the list is sorted in ascending order by their angle from the segemnt measured in radian, clockwise. </returns>
		private List<int> FindPreviousSegmentsIndexSortedByAngle(int segmentIndex, List<LineSegment> pool)
		{
			var connectedSegmentIndexes = new List<int>();
			for (int i = 0; i< pool.Count; i++)
			{ 
				if (i != segmentIndex)
				{
					if (pool[segmentIndex].FirstPoint == pool[i].FirstPoint || pool[segmentIndex].FirstPoint == pool[i].SecondPoint)
					{
						connectedSegmentIndexes.Add(i);
					}
				}
				
			}
		}

		/// <summary>
		/// Find the indexes of segments connected to the SecondPoint of the segment from the pool, sorted by their angle from the segment
		/// </summary>
		/// <param name="segmentIndex"> the index of the segment from the pool </param>
		/// <param name="pool"> the list of segments to search from </param>
		/// <returns> the list of indexes of segments connected to the SecondPoint of the input segment; 
		/// the list is sorted in ascending order by their angle from the segemnt measured in radian, clockwise. </returns>
		private List<int> FindNextSegmentsIndexSortedByAngle(int segmentIndex, List<LineSegment> pool)
		{
			
		}

		class LinkedLineSegment
		{
			internal LineSegment WrappedLineSegment { get; }
			internal List<LinkedLineSegment> PreviousSegments { get; private set; } = new List<LinkedLineSegment>();
			internal List<LinkedLineSegment> NextSegments { get; private set; } = new List<LinkedLineSegment>();
			internal bool IsDangling => (this.PreviousSegments.Count == 0 || this.NextSegments.Count == 0);

			internal LinkedLineSegment(LineSegment ls)
			{
				this.WrappedLineSegment = ls;
			}

			internal void SortPreviousAndNextSegmentsListsByAngle()
			{
				this.PreviousSegments = this.PreviousSegments.OrderBy(o => 
				{
					Point p1 = this.WrappedLineSegment.SecondPoint;
					Point p2 = this.WrappedLineSegment.FirstPoint;
					Point p3;

					if (p2 == o.WrappedLineSegment.FirstPoint)
					{
						p3 = o.WrappedLineSegment.FirstPoint;
					}
					else
					{
						Debug.Assert(p2 == o.WrappedLineSegment.SecondPoint);
						p3 = o.WrappedLineSegment.SecondPoint;
					}
					return GeometryUtilities.AngleAmongThreePoints(p1, p2, p3);
				}).ToList();

				this.NextSegments = this.NextSegments.OrderBy(o =>
				{
					Point p1 = this.WrappedLineSegment.FirstPoint;
					Point p2 = this.WrappedLineSegment.SecondPoint;
					Point p3;

					if (p2 == o.WrappedLineSegment.FirstPoint)
					{
						p3 = o.WrappedLineSegment.FirstPoint;
					}
					else
					{
						Debug.Assert(p2 == o.WrappedLineSegment.SecondPoint);
						p3 = o.WrappedLineSegment.SecondPoint;
					}
					return GeometryUtilities.AngleAmongThreePoints(p1, p2, p3);
				}).ToList();
			}
		}
	}
}
