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

		//internal List<EnclosedProgram> FindRooms()
		//{
		//	do
		//	{
		//		var segments = this.FragmentsLeftToResolve.Pop();
		//		// Find the segment with the left most starting point, which is guarantted to be a boundary point. 
		//		List<LineSegment> segmentsWithLeftmostFirstPoint = new List<LineSegment> { segments[0] };
		//		for (int i = 1; i< segments.Count; i++)
		//		{
		//			if(segments[i].FirstPoint.coordinateX < segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
		//			{
		//				// this one's first point is even more on the left
		//				segmentsWithLeftmostFirstPoint = new List<LineSegment> { segments[i] };
		//			} 
		//			else if (segments[i].FirstPoint.coordinateX == segmentsWithLeftmostFirstPoint.First().FirstPoint.coordinateX)
		//			{
		//				// this one's first point is also on the far left
		//				segmentsWithLeftmostFirstPoint.Add(segments[i]);
		//			}
		//		}

		//		// Initialize the starting handle
		//		var startPoint = segmentsWithLeftmostFirstPoint.First().FirstPoint;
		//		LineSegment startingHandle = new LineSegment(new Point(startPoint.coordinateX, startPoint.coordinateY - 10), startPoint);

		//		bool IsFirstPointTheOneToSearch = false; // for the first segment the first point is connected to the handel
		//		var currentSegment = startingHandle;

		//		// TODO: refactor to deal with dangling segments first! 
		//		// TODO: copy segments before removing

		//		// Find a perimeter by keeping turning the largest angle
		//		var perimeter = new List<LineSegment>();
		//		LineSegment nextSegment;
		//		do
		//		{
		//			perimeter.Add(currentSegment);

		//			// find the next segment
		//			if (IsFirstPointTheOneToSearch)
		//			{
		//				var result = SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, segments);
		//				if (result.Count > 0)
		//				{
		//					nextSegment = result.Last(); // choose the one with largest angle
		//					IsFirstPointTheOneToSearch = !(currentSegment.FirstPoint == nextSegment.FirstPoint);
							
		//					currentSegment = nextSegment;
		//				}
		//				else
		//				{
		//					// this is a dangling segment and cannot be part of any room. Remove it from the pool
		//					segments.Remove(currentSegment);
		//					perimeter.Remove(currentSegment);
		//					if (perimeter.Count == 0)
		//					{
		//						break; // the segments lead to nowhere; not finding any room from this handle
		//					}
		//					// roll back
		//					currentSegment = perimeter.Last();
		//					perimeter.RemoveAt(perimeter.Count - 1);
		//				}
		//			}
		//			else
		//			{
		//				var result = SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, segments);
		//				if (result.Count > 0)
		//				{
		//					nextSegment = result.Last(); // choose the one with largest angle
		//					IsFirstPointTheOneToSearch = !(currentSegment.SecondPoint == nextSegment.FirstPoint);
							
		//					currentSegment = nextSegment;
		//				}
		//				else
		//				{
		//					// this is a dangling segment and cannot be part of any room. Remove it
		//					segments.Remove(currentSegment);
		//					perimeter.Remove(currentSegment);
		//					if (perimeter.Count == 0)
		//					{
		//						break; // not finding any room from this handle
		//					}
		//					// roll back
		//					currentSegment = perimeter.Last();
		//					perimeter.RemoveAt(perimeter.Count - 1);
		//				}
		//			}
		//		} while (!perimeter.Contains(currentSegment));

		//		perimeter.RemoveAt(0); // remove the handle! 

		//		if (perimeter.Count > 2)
		//		{
		//			// found a perimeter! 
		//			var cycle = new CycleOfLineSegments(perimeter);

		//			// TODO: make a DaigramFragment from the perimeter and all included segments and push into a stack. Remove all these segments from FragmentsLeftToResolve
		//		}
		//	} while (this.FragmentsLeftToResolve.Count > 0);

			
			
			
			
			
		//}
	
	}
}
