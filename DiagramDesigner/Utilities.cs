﻿using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;
using DiagramDesignerEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("DiagramDesigner.UnitTests")]
namespace DiagramDesigner
{
	static class Utilities
	{
		public static WinPoint ConvertPointToWindowsPoint(Point p, double winPointUnitOverPointUnit)
		{
			return new WinPoint(p.coordinateX * winPointUnitOverPointUnit, p.coordinateY * winPointUnitOverPointUnit);
		}

		public static  Point ConvertWindowsPointToPoint (WinPoint wp, double winPointUnitOverPointUnit)
		{
			return new Point(wp.X / winPointUnitOverPointUnit, wp.Y / winPointUnitOverPointUnit);
		}

		public static double DistanceBetweenWinPoints(WinPoint p1, WinPoint p2)
		{
			var diffX = p1.X - p2.X;
			var diffY = p1.Y - p2.Y;
			return Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
		}

		/// <summary>
		/// Projects the point to the line and find the distance between the point and its projection
		/// source: https://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
		/// </summary>
		/// <returns> The distance if projection is possible, otherwise null </returns>
		public static Tuple<double, WinPoint> DistanceFromWinPointToLine(WinPoint wp, WinPoint endPoint1, WinPoint endPoint2)
		{
			var A = wp.X - endPoint1.X;
			var B = wp.Y - endPoint1.Y;
			var C = endPoint2.X - endPoint1.X;
			var D = endPoint2.Y - endPoint1.Y;

			var dot = A * C + B * D;
			var len_sq = C * C + D * D;
			double param = -1;
			if (len_sq == 0)
			{
				throw new ArgumentException("length of line segment is 0");
			}
			param = dot / len_sq;

			if (param >= 0 && param <= 1)
			{
				var projX = endPoint1.X + param * C;
				var projY = endPoint1.Y + param * D;
				var projWinPoint = new WinPoint(projX, projY);

				return new Tuple<double, WinPoint>(Utilities.DistanceBetweenWinPoints(wp, projWinPoint), projWinPoint);
			} 
			else
			{
				return null;
			}
		}
	}
}
