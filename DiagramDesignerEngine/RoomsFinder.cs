using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class RoomsFinder
	{
		private List<LinkedLineSegment> LinkedLineSegments;
		private Stack<List<LinkedLineSegment>> FragmentsLeftToResolve;

		internal RoomsFinder(List<LineSegment> explodedSegments)
		{
			this.LinkedLineSegments = new List<LinkedLineSegment>();

			// convert all segments to linked segments objects
			foreach (LineSegment ls in explodedSegments)
			{
				this.LinkedLineSegments.Add(new LinkedLineSegment(ls));	
			}

			// fill the previous and next segments lists of all linked segments
			for (int i = 0; i < this.LinkedLineSegments.Count; i++)
			{
				for (int j = i + 1; j < this.LinkedLineSegments.Count; j++)
				{
					if (this.LinkedLineSegments[i].WrappedLineSegment.FirstPoint == this.LinkedLineSegments[j].WrappedLineSegment.FirstPoint) 
					{
						this.LinkedLineSegments[i].PreviousSegments.Add(this.LinkedLineSegments[j]);
						this.LinkedLineSegments[j].PreviousSegments.Add(this.LinkedLineSegments[i]);
					}

					if (this.LinkedLineSegments[i].WrappedLineSegment.FirstPoint == this.LinkedLineSegments[j].WrappedLineSegment.SecondPoint)
					{
						this.LinkedLineSegments[i].PreviousSegments.Add(this.LinkedLineSegments[j]);
						this.LinkedLineSegments[j].NextSegments.Add(this.LinkedLineSegments[i]);
					}

					if (this.LinkedLineSegments[i].WrappedLineSegment.SecondPoint == this.LinkedLineSegments[j].WrappedLineSegment.FirstPoint)
					{
						this.LinkedLineSegments[i].NextSegments.Add(this.LinkedLineSegments[j]);
						this.LinkedLineSegments[j].PreviousSegments.Add(this.LinkedLineSegments[i]);
					}
					if (this.LinkedLineSegments[i].WrappedLineSegment.SecondPoint == this.LinkedLineSegments[j].WrappedLineSegment.SecondPoint)
					{
						this.LinkedLineSegments[i].NextSegments.Add(this.LinkedLineSegments[j]);
						this.LinkedLineSegments[j].NextSegments.Add(this.LinkedLineSegments[i]);
					}
				}
			}

			// sort?? 


			// init resolution stack with exploded segments
			this.FragmentsLeftToResolve = new Stack<List<LinkedLineSegment>>();
			this.FragmentsLeftToResolve.Push(this.LinkedLineSegments);
		}

		internal List<EnclosedProgram> FindRooms()
		{
			do
			{
				var segments = this.FragmentsLeftToResolve.Pop();
				// Find the segment with the left most starting point, which is guarantted to be a boundary point. 
				LinkedLineSegment nonDanglingSegmentWithLeftmostFirstPoint = null;
				foreach (LinkedLineSegment lls in segments)
				{
					if (lls.IsDangling)
					{
						continue;
					}

					if (nonDanglingSegmentWithLeftmostFirstPoint == null)
					{
						nonDanglingSegmentWithLeftmostFirstPoint = lls;
					}
					else if(lls.WrappedLineSegment.FirstPoint.coordinateX < nonDanglingSegmentWithLeftmostFirstPoint.WrappedLineSegment.FirstPoint.coordinateX)
					{
						// this one's first point is even more on the left
						nonDanglingSegmentWithLeftmostFirstPoint = lls;
					}
				}
				if (nonDanglingSegmentWithLeftmostFirstPoint is null)
				{
					// there can't be any room if every exploded edge is dangling
					return new List<EnclosedProgram>();
				}

				// Find a perimeter by keeping turning the smallest angle
				


			} while (this.FragmentsLeftToResolve.Count > 0);


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
					return GeometryUtilities.AngleBetweenConnectedSegments(p1, p2, p3);
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
					return GeometryUtilities.AngleBetweenConnectedSegments(p1, p2, p3);
				}).ToList();
			}
		}
	}
}
