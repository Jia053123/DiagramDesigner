using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	/// <summary>
	/// Manage and apply the drafting constrains such as snapping and aligning. 
	/// </summary>
	class DraftingController
	{
		private List<List<WinPoint>> CurrentGeometries;
		internal bool IsDrawingOrthogonally = false;
		internal WinPoint? LastAddedPointInEditingState = null;

		internal DraftingController(List<List<WinPoint>> currentGeometries)
		{
			this.CurrentGeometries = currentGeometries;
		}

		internal void UpdateGeometries(List<List<WinPoint>> newGeometries)
		{
			this.CurrentGeometries = newGeometries;
		}

		internal void ClearLastAddedPoint()
		{
            this.LastAddedPointInEditingState = null;
		}

		internal WinPoint ApplyAllRestrictions(WinPoint newP)
		{
			WinPoint orthoP = this.ApplyOrthogonalRestrictions(newP);
			WinPoint snappedP = this.SnapToPointOrLineNearby(orthoP);
			return snappedP;
		}

		private WinPoint ApplyOrthogonalRestrictions(WinPoint newP)
		{
			if (this.IsDrawingOrthogonally)
			{
				if (!(this.LastAddedPointInEditingState is null))
				{
					return MathUtilities.PointOrthogonal((WinPoint)this.LastAddedPointInEditingState, newP);
				}
			}
			return newP;
		}

		private WinPoint SnapToPointOrLineNearby(WinPoint newP)
		{
			// snap to point or line nearby
			var pointCloseBy = this.FindPointCloseBy(newP);
			var pointOnLineCloseBy = this.FindPointOnLine(newP);
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
		/// Find a Windows Point close by measured by pixel. 
		/// This is intended for the UI feature that snap new points to existing points close by.
		/// </summary>
		/// <param name="wPoint"> The new point </param>
		/// <returns> A point in WallEntity on screen in close proximity, or null if no existing point qualifies </returns>
		private WinPoint? FindPointCloseBy(WinPoint wPoint)
		{
			const double maxDistance = 5;
			for (int i = 0; i < this.CurrentGeometries.Count; i++)
			{
				for (int j = 0; j < this.CurrentGeometries[i].Count; j++)
				{
					var p = this.CurrentGeometries[i][j];
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
		/// <returns> A point on a line segment on screen in close proximity, or null if no line qualifies </returns>
		private WinPoint? FindPointOnLine(WinPoint wPoint)
		{
			const double maxDistance = 5;
			for (int i = 0; i < this.CurrentGeometries.Count; i++)
			{
				for (int j = 0; j < this.CurrentGeometries[i].Count - 1; j++)
				{
					var endPoint1 = this.CurrentGeometries[i][j];
					var endPoint2 = this.CurrentGeometries[i][j + 1];
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
