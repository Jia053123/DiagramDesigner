using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
	/// <summary>
	/// Manage and apply the drafting constrains such as snapping and aligning. 
	/// </summary>
	class DraftingConstrainsApplier
	{
		public bool IsDrawingOrthogonally;
		private List<List<WinPoint>> CurrentGeometries;

		public DraftingConstrainsApplier(List<List<WinPoint>> currentGeometries, bool drawOrthogonally)
		{
			this.CurrentGeometries = currentGeometries;
			this.IsDrawingOrthogonally = drawOrthogonally;
		}
	}
}
