using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	/// <summary>
	/// Manage and apply the drafting constrains such as snapping and aligning. 
	/// </summary>
	class DraftingManager
	{
		public bool IsDrawingOrthogonally = false;
		private List<List<WinPoint>> CurrentGeometries;

		public DraftingManager(List<List<WinPoint>> currentGeometries)
		{
			this.CurrentGeometries = currentGeometries;
		}

		//protected WinPoint ApplyOrthogonalRestrictions(WinPoint newP)
		//{
		//	if (this.IsDrawingOrthogonally)
		//	{
		//		if (!(this.LastAddedPointInRuleCreationEditingState is null))
		//		{
		//			return MathUtilities.PointOrthogonal((WinPoint)this.LastAddedPointInRuleCreationEditingState, newPoint);
		//		}
		//	} else
		//	{
		//		return newP;
		//	}
			
		//}
	}
}
