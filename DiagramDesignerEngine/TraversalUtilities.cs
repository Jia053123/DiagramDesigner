using BasicGeometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesignerEngine
{
	static class TraversalUtilities
	{
		/// <summary>
		/// Find the angle between two connected line segments, from the first one to the second, clockwise
		/// </summary>
		/// <param name="ls1"> the first segment </param>
		/// <param name="ls2"> the second segment that's connected to the first one </param>
		/// <returns> The angle between 0 and 2pi by going from the first segment, through the shared end point to the second one
		/// The angle is measured clockwise; return 0 if the segments overlap </returns>
		internal static double AngleAmongTwoSegments(LineSegment ls1, LineSegment ls2)
		{
			if (ls1.FirstPoint == ls2.FirstPoint)
			{
				return TraversalUtilities.AngleAmongThreePoints(ls1.SecondPoint, ls1.FirstPoint, ls2.SecondPoint);
			}
			else if (ls1.FirstPoint == ls2.SecondPoint)
			{
				return TraversalUtilities.AngleAmongThreePoints(ls1.SecondPoint, ls1.FirstPoint, ls2.FirstPoint);
			}
			else if (ls1.SecondPoint == ls2.FirstPoint)
			{
				return TraversalUtilities.AngleAmongThreePoints(ls1.FirstPoint, ls1.SecondPoint, ls2.SecondPoint);
			}
			else if (ls1.SecondPoint == ls2.SecondPoint)
			{
				return TraversalUtilities.AngleAmongThreePoints(ls1.FirstPoint, ls1.SecondPoint, ls2.FirstPoint);
			}
			else
			{
				throw new ArgumentException("The two segments are not connected");
			}
		}

		/// <summary>
		/// Find the angle between two connected line segments represented by three points
		/// </summary>
		/// <param name="startPoint"> the end point of the first segment marking the beginning of the angle </param>
		/// <param name="sharedPoint"> the end point shared by the two segments </param>
		/// <param name="endPoint"> the end point of the second segment marking the end of the angle </param>
		/// <returns> The angle between 0 and 2pi by going from startPoint, through sharedPoint to endPoint. 
		/// The angle is measured clockwise; return 0 if the segments overlap </returns>
		internal static double AngleAmongThreePoints(Point startPoint, Point sharedPoint, Point endPoint)
		{
			// move all points together so that sharedPoint is at 0,0
			var translatedStartPoint = new Point(startPoint.coordinateX - sharedPoint.coordinateX,
				startPoint.coordinateY - sharedPoint.coordinateY);
			var translatedEndPoint = new Point(endPoint.coordinateX - sharedPoint.coordinateX,
				endPoint.coordinateY - sharedPoint.coordinateY);

			// get the angle from positive x axis with tan
			var angleForStartPoint = Math.Atan2(translatedStartPoint.coordinateY, translatedStartPoint.coordinateX);
			var angleForEndPoint = Math.Atan2(translatedEndPoint.coordinateY, translatedEndPoint.coordinateX);
			var angleDiff = angleForStartPoint - angleForEndPoint;

			// Math.Atan2 return value is between -pi and pi. Map to output
			if (angleDiff <= 0)
			{
				angleDiff += Math.PI * 2;
			}

			if (angleDiff > Math.PI * 2)
			{
				angleDiff -= Math.PI * 2;
			}

			return angleDiff;
		}

		/// <summary>
		/// Sort list of segments by their angle from a segment in ascending order
		/// </summary>
		/// <param name="ls"> the segment where the angle starts </param>
		/// <param name="connectedLs"> a list of segments connected to the segment </param>
		/// <returns> segments sorted by angle in ascending order </returns>
		private static List<LineSegment> SortSegmentsByAngleFromSegment(LineSegment ls, List<LineSegment> connectedLs)
		{	
			return connectedLs.OrderBy(o => TraversalUtilities.AngleAmongTwoSegments(ls, o)).ToList();
		}

		/// <summary>
		/// Find the indexes of segments connected to the FirstPoint of the segment from the pool, sorted by their angle from the segment
		/// </summary>
		/// <param name="segmentIndex"> the beginning segment </param>
		/// <param name="segmentsPool"> the list of segments to search from </param>
		/// <returns> the list of segments from the pool connected to the FirstPoint of the segment, 
		/// excluding segments equal to the input segment; 
		/// the list is sorted in ascending order by their angle from the beginning segemnt measured clockwise. </returns>
		internal static List<LineSegment> FindLeftConnectedSegmentsSortedByAngle(LineSegment segment, List<LineSegment> segmentsPool)
		{
			var connectedSegments = new List<LineSegment>();
			for (int i = 0; i < segmentsPool.Count; i++)
			{
				if (segmentsPool[i] != segment)
				{
					if (segment.FirstPoint == segmentsPool[i].FirstPoint || segment.FirstPoint == segmentsPool[i].SecondPoint)
					{
						connectedSegments.Add(segmentsPool[i]);
					}
				}
			}
			return TraversalUtilities.SortSegmentsByAngleFromSegment(segment, connectedSegments);
		}

		/// <summary>
		/// Find the indexes of segments connected to the SecondPoint of the segment from the pool, sorted by their angle from the segment
		/// </summary>
		/// <param name="segmentIndex"> the index of the segment from the pool </param>
		/// <param name="pool"> the list of segments to search from </param>
		/// <returns> the list of indexes of segments connected to the SecondPoint of the input segment, 
		/// excluding segments equal to the input segment; 
		/// the list is sorted in ascending order by their angle from the segemnt measured in radian, clockwise. </returns>
		internal static List<LineSegment> FindRightConnectedSegmentsSortedByAngle(LineSegment segment, List<LineSegment> segmentsPool)
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
			return TraversalUtilities.SortSegmentsByAngleFromSegment(segment, connectedSegments);
		}

		/// <summary>
		/// Remove LineSegment items that are not connected on both ends. Duplicate segments are not connected to each other
		/// </summary>
		/// <param name="lineSegments"> The segments to search for dangling segments </param>
		/// <returns> The segments with dangling segments removed and have none left, otherwise in the same order </returns>
		internal static List<LineSegment> RemoveDanglingLineSegments(List<LineSegment> lineSegments)
		{
			List<LineSegment> segmentsOfCurrentIteration;
			var segmentsOfNextIteration = new List<LineSegment>(lineSegments); // make a copy
			bool oneSegmentRemoved;
			do
			{
				oneSegmentRemoved = false;
				segmentsOfCurrentIteration = new List<LineSegment>(segmentsOfNextIteration);
				for (int i = 0; i < segmentsOfCurrentIteration.Count; i++)
				{
					var ls = segmentsOfCurrentIteration[i];
					var leftResult = TraversalUtilities.FindLeftConnectedSegmentsSortedByAngle(ls, segmentsOfCurrentIteration);
					var rightResult = TraversalUtilities.FindRightConnectedSegmentsSortedByAngle(ls, segmentsOfCurrentIteration);

					if (leftResult.Count == 0 || rightResult.Count == 0)
					{
						segmentsOfNextIteration.Remove(ls); // RemoveAt may remove the wrong one if segments have been removed before that index
						oneSegmentRemoved = true;
					}
				}
			} while (oneSegmentRemoved);
			
			return segmentsOfNextIteration;
		}
	}
}
