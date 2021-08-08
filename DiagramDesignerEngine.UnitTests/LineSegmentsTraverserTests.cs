using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class LineSegmentsTraverserTests
	{
		[Test]
		public void TestTraverseSegments_1()
		{
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls4 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var segments1 = new List<LineSegment> { ls1, ls3, ls2, ls4 };

			// basic perfect loop case
			var traverser1 = new LineSegmentsTraverser(segments1);
			var result1 = traverser1.TraverseSegments(ls1, true, true);
			Assert.AreEqual(result1, 0);
			Assert.AreEqual(traverser1.GetPath().Count, 4);
			foreach (LineSegment ls in segments1)
			{
				Assert.IsTrue(traverser1.GetPath().Contains(ls));
			}

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, -2));
			var segments2 = new List<LineSegment> { ls1, ls4, ls2, ls3, ls5 };

			// starting handle case
			var traverser2 = new LineSegmentsTraverser(segments2);
			var result2 = traverser2.TraverseSegments(ls5, false, false);
			Assert.AreEqual(result2, 1);
			Assert.AreEqual(traverser2.GetPath().Count, 5);
			foreach (LineSegment ls in segments2)
			{
				Assert.IsTrue(traverser2.GetPath().Contains(ls));
			}
		}
	}
}
