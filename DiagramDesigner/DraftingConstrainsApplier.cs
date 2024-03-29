﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	/// <summary>
	/// Assist the selecting and drafting operations to take into consideration proximity and constrains such as snapping and aligning. 
	/// </summary>
	class DraftingConstrainsApplier
	{
		internal bool DoesDrawOrthogonally = false;
		private WinPoint? LastAddedPointInEditingState = null;

		internal void UpdateLastAddedPoint(WinPoint newP)
		{
			this.LastAddedPointInEditingState = newP;
		}
	
		/// <summary>
		/// To be called whenever a polyline drawing is complete
		/// </summary>
		internal void ClearLastAddedPoint()
		{
            this.LastAddedPointInEditingState = null;
		}

		internal WinPoint ApplyAllRestrictions(WinPoint newP, List<List<WinPoint>> currentGeometries)
		{
			// first apply ortho if enabled, then snap to close elements
			WinPoint orthoP = this.ApplyOrthogonalRestrictions(newP);
			WinPoint snappedP = this.SnapToPointOrLineNearby(orthoP, currentGeometries);

			return snappedP;
		}

		internal WinPoint ApplyOrthogonalRestrictions(WinPoint newP)
		{
			if (this.DoesDrawOrthogonally)
			{
				if (!(this.LastAddedPointInEditingState is null))
				{
					return MathUtilities.PointOrthogonal((WinPoint)this.LastAddedPointInEditingState, newP);
				}
			}
			return newP;
		}

		internal WinPoint SnapToPointOrLineNearby(WinPoint newP, List<List<WinPoint>> currentGeometries)
		{
			// snap to point or line nearby
			var pointCloseBy = this.FindPointCloseBy(newP, currentGeometries);
			var pointOnLineCloseBy = this.FindPointOnLine(newP, currentGeometries);
			if (!(pointCloseBy is null))
			{
				// prioritize pointCloseBy
				return (WinPoint)pointCloseBy;
			}
			else if (!(pointOnLineCloseBy is null))
			{
				return (WinPoint)pointOnLineCloseBy;
			}
			else
			{
				return newP;
			}
		}

		/// <summary>
		/// Find the line segment on screen clicked
		/// </summary>
		/// <param name="clickLocation"> location of the click </param>
		/// <param name="currentGeometries"> Existing geometries to consider </param>
		/// <returns> a tuple containing the index of the geometry, 
		/// the two consecutive indexes in ascending order of the points representing the line on the geometry, 
		/// or null if no line is clicked </returns>
		internal Tuple<int, int, int> FindLineClicked(WinPoint clickLocation, List<List<WinPoint>> currentGeometries)
		{
			const double tolerance = 2;
			for (int i = 0; i < currentGeometries.Count; i++)
			{
				for (int j = 0; j < currentGeometries[i].Count - 1; j++)
				{
					var endPoint1 = currentGeometries[i][j];
					var endPoint2 = currentGeometries[i][j + 1];
					var result = MathUtilities.DistanceFromWinPointToLine(clickLocation, endPoint1, endPoint2);
					if (!(result is null) && result.Item1 <= tolerance)
					{
						Debug.Assert(j + 1 < currentGeometries[i].Count);
						return new Tuple<int, int, int>(i, j, j + 1);
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Find a Windows Point close by measured by pixel. 
		/// This is intended for the UI feature that snap new points to existing points close by.
		/// </summary>
		/// <param name="wPoint"> The new point </param>
		/// <param name="currentGeometries"> Existing geometries to consider </param>
		/// <returns> A point in WallEntity on screen in close proximity, or null if no existing point qualifies </returns>
		private WinPoint? FindPointCloseBy(WinPoint wPoint, List<List<WinPoint>> currentGeometries)
		{
			const double maxDistance = 5;
			for (int i = 0; i < currentGeometries.Count; i++)
			{
				for (int j = 0; j < currentGeometries[i].Count; j++)
				{
					var p = currentGeometries[i][j];
					if (MathUtilities.DistanceBetweenWinPoints(wPoint, p) <= maxDistance)
					{
						return p;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Find a Windows Point on a line close by measured by pixel. 
		/// This is intended for the UI feature that snap new points to existing lines close by. 
		/// </summary>
		/// <param name="wPoint"> The new point </param>
		/// <param name="currentGeometries"> Existing geometries to consider </param>
		/// <returns> A point on a line segment on screen in close proximity, or null if no line qualifies </returns>
		private WinPoint? FindPointOnLine(WinPoint wPoint, List<List<WinPoint>> currentGeometries)
		{
			const double maxDistance = 5;
			for (int i = 0; i < currentGeometries.Count; i++)
			{
				for (int j = 0; j < currentGeometries[i].Count - 1; j++)
				{
					var endPoint1 = currentGeometries[i][j];
					var endPoint2 = currentGeometries[i][j + 1];
					var result = MathUtilities.DistanceFromWinPointToLine(wPoint, endPoint1, endPoint2);
					if (!(result is null) && result.Item1 <= maxDistance)
					{
						return result.Item2;
					}
				}
			}
			return null;
		}
	}
}
