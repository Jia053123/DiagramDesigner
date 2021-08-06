using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
	static class GeometryUtilities
	{
		/// <summary>
		/// Find the angle between two connected line segments
		/// </summary>
		/// <param name="ls1"> the first segment </param>
		/// <param name="ls2"> the second segment that's connected to the first one </param>
		/// <returns> The angle between 0 and 2pi by going from the first segment, through the shared end point to the second one
		/// The angle is measured clockwise; return 0 if the segments overlap </returns>
		internal static double AngleAmongTwoSegments(LineSegment ls1, LineSegment ls2)
		{
			if (ls1.FirstPoint == ls2.FirstPoint)
			{
				return GeometryUtilities.AngleAmongThreePoints(ls1.SecondPoint, ls1.FirstPoint, ls2.SecondPoint);
			}
			else if (ls1.FirstPoint == ls2.SecondPoint)
			{
				return GeometryUtilities.AngleAmongThreePoints(ls1.SecondPoint, ls1.FirstPoint, ls2.FirstPoint);
			}
			else if (ls1.SecondPoint == ls2.FirstPoint)
			{
				return GeometryUtilities.AngleAmongThreePoints(ls1.FirstPoint, ls1.SecondPoint, ls2.SecondPoint);
			}
			else if (ls1.SecondPoint == ls2.SecondPoint)
			{
				return GeometryUtilities.AngleAmongThreePoints(ls1.FirstPoint, ls1.SecondPoint, ls2.FirstPoint);
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
			if (angleDiff < 0)
			{
				angleDiff += Math.PI * 2;
			}

			if (angleDiff >= Math.PI * 2)
			{
				angleDiff -= Math.PI * 2;
			}

			return angleDiff;
		}
	}
}
