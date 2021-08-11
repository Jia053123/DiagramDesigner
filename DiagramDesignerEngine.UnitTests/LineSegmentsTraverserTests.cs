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
			//      __
			//     |__|
			//     
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls4 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var segments1 = new List<LineSegment> { ls1, ls3, ls2, ls4 };

			// basic perfect loop case
			var traverser1 = new LineSegmentsTraverser(segments1);
			var result1 = traverser1.TraverseSegments(ls1, true, true);
			Assert.AreEqual(result1.Item1, 0);
			Assert.AreEqual(result1.Item2.Count, 4);
			Assert.AreEqual(result1.Item2.Count+1, traverser1.GetLastPointsAlongPath().Count);
			foreach (LineSegment ls in segments1)
			{
				Assert.IsTrue(traverser1.GetLastPath().Contains(ls));
			}

			//      __
			//     |__|
			//     |
			//
			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, -2));
			var segments2 = new List<LineSegment> { ls1, ls4, ls2, ls3, ls5 };

			// starting handle case
			var traverser2 = new LineSegmentsTraverser(segments2);
			var result2 = traverser2.TraverseSegments(ls5, false, false);
			Assert.AreEqual(result2.Item1, 1);
			Assert.AreEqual(result2.Item2.Count, 5);
			Assert.AreEqual(result2.Item2.Count + 1, traverser2.GetLastPointsAlongPath().Count);
			foreach (LineSegment ls in segments2)
			{
				Assert.IsTrue(traverser2.GetLastPath().Contains(ls));
			}

			// reach the dead end if pick the wrong point to start
			var result21 = traverser2.TraverseSegments(ls5, true, false);
			Assert.AreEqual(result21.Item1, -1);
			Assert.AreEqual(result21.Item2.Count, 1);
			Assert.AreEqual(result21.Item2.Count + 1, traverser2.GetLastPointsAlongPath().Count);
			Assert.IsTrue(result21.Item2.Contains(ls5));

			//      __
			//     |__
			//     |
			//

			// test angle
			var segments3 = new List<LineSegment> { ls1, ls4, ls2, ls5 };
			var traverser3 = new LineSegmentsTraverser(segments3);
			var result3 = traverser3.TraverseSegments(ls5, false, false);
			Assert.AreEqual(result3.Item1, -1);
			Assert.AreEqual(result3.Item2.Count, 3);
			Assert.AreEqual(result3.Item2.Count + 1, traverser3.GetLastPointsAlongPath().Count);
			Assert.AreEqual(result3.Item2[0], ls5);
			Assert.AreEqual(result3.Item2[1], ls1);
			Assert.AreEqual(result3.Item2[2], ls2);

			var result31 = traverser3.TraverseSegments(ls5, false, true);
			Assert.AreEqual(result31.Item1, -1);
			Assert.AreEqual(result31.Item2.Count, 2);
			Assert.AreEqual(result31.Item2.Count + 1, traverser3.GetLastPointsAlongPath().Count);
			Assert.AreEqual(result31.Item2[0], ls5);
			Assert.AreEqual(result31.Item2[1], ls4);
		}

		[Test]
		public void TestTraverseSegments_DepthFirstSearch()
		{
			// 3
			//        |
			// 2 _____|_____
			//        |     |
			// 1      |_____|_____
			//              |
			// 0            |
			//
			//  0     1     2     3

			var ls1 = new LineSegment(new Point(0, 2), new Point(1, 2));
			var ls2 = new LineSegment(new Point(1, 2), new Point(1, 3));
			var ls3 = new LineSegment(new Point(1, 2), new Point(2, 2));
			var ls4 = new LineSegment(new Point(1, 2), new Point(1, 1));
			var ls5 = new LineSegment(new Point(1, 1), new Point(2, 1));
			var ls6 = new LineSegment(new Point(2, 1), new Point(2, 2));
			var ls7 = new LineSegment(new Point(2, 1), new Point(3, 1));
			var ls8 = new LineSegment(new Point(2, 1), new Point(2, 0));

			var segments = new List<LineSegment> { ls1, ls8, ls2, ls7, ls3, ls6, ls4, ls5 };
			var traverser = new LineSegmentsTraverser(segments);
			Assert.Throws<InvalidOperationException>(() => traverser.TraverseAgain());

			var result1 = traverser.TraverseSegments(ls1, false, true);
			var expectedPath1 = new List<LineSegment> { ls1, ls4, ls5, ls8 };
			var expectedPoints1 = new List<Point> { new Point(0, 2), new Point(1, 2), new Point(1, 1), new Point(2, 1), new Point(2, 0) };
			Assert.AreEqual(-1, result1.Item1);
			Assert.IsTrue(TestUtilities.AreContentsEqualInOrder(result1.Item2, expectedPath1));
			Assert.IsTrue(TestUtilities.AreContentsEqualInOrder(traverser.GetLastPath(), expectedPath1));
			Assert.IsTrue(TestUtilities.AreContentsEqualInOrder(traverser.GetLastPointsAlongPath(), expectedPoints1));

			//var result2 = traverser.TraverseAgain();
			//var expectedPath2 = new List<LineSegment> { ls1, ls4, ls5, ls7 };
			//Assert.AreEqual(-1, result2.Item1);
			//Assert.IsTrue(TestUtilities.AreContentsEqualInOrder(result2.Item2, expectedPath2));
		}
	}
}
