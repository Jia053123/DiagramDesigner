using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
	class RoomsFinder
	{
		private List<LineSegment> LineSegments;
		private Stack<List<LineSegment>> FragmentsLeftToResolve;

		internal RoomsFinder(List<LineSegment> explodedSegments)
		{
			this.LineSegments = explodedSegments;
			// init resolution stack with exploded segments
			this.FragmentsLeftToResolve = new Stack<List<LineSegment>>();
			this.FragmentsLeftToResolve.Push(explodedSegments);
		}

		//internal List<EnclosedProgram> FindRooms()
		//{
		//	do
		//	{
		//		var segments = this.FragmentsLeftToResolve.Pop();
		//		// starting from the left most point, which is guarantted to be a boundary point. Find a perimeter by keeping turning the smallest angle

				
		//	} while (this.FragmentsLeftToResolve.Count > 0);


		//}


		//private List<int> FindTheNextSegments(int startingSegmentIndex, List<LineSegment> segmentsPool)
		//{
			
		//}

		//private List<int> FindThePreviousSegments(int startingSegmentIndex, List<LineSegment> segmentsPool)
		//{

		//}

		
	}
}
