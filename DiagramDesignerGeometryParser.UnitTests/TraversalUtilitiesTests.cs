using BasicGeometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DiagramDesignerGeometryParser.UnitTests
{
	class TraversalUtilitiesTests
	{
		[Test]
		public void TestAngleAmongTwoSegments()
		{
			var p0 = new Point(0, 0);
			var p1 = new Point(2, 0);
			var p2 = new Point(-2, 0);
			var p3 = new Point(0, 1);
			var p4 = new Point(0, -1.5);

			var ls1 = new LineSegment(p1, p0);
			var ls2 = new LineSegment(p2, p0);
			var ls3 = new LineSegment(p3, p0);
			var ls4 = new LineSegment(p4, p0);

			Assert.AreEqual(TraversalUtilities.AngleAmongTwoSegments(ls1, ls2), Math.PI);
			Assert.AreEqual(TraversalUtilities.AngleAmongTwoSegments(ls2, ls1), Math.PI);

			Assert.AreEqual(TraversalUtilities.AngleAmongTwoSegments(ls1, ls3), Math.PI * 1.5);
			Assert.AreEqual(TraversalUtilities.AngleAmongTwoSegments(ls3, ls1), Math.PI * 0.5);

			Assert.AreEqual(TraversalUtilities.AngleAmongTwoSegments(ls1, ls4), Math.PI * 0.5);
			Assert.AreEqual(TraversalUtilities.AngleAmongTwoSegments(ls4, ls1), Math.PI * 1.5);

			Assert.AreEqual(TraversalUtilities.AngleAmongTwoSegments(ls1, ls1), Math.PI * 2.0);
		}

		[Test]
		public void TestFindLeftConnectedSegmentsSortedByAngle()
		{
			//
			//    \       /
			//     \_____/___  ____
			//     /      \   
			//    /        \___
			//
			// -1.5 -1   0 0.5 1   2
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1.5, 1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1.5, 1));
			var ls3 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(0.5, 1));
			var ls5 = new LineSegment(new Point(0, 0), new Point(0.5, -1));
			var ls6 = new LineSegment(new Point(0.5, -1), new Point(1, -1));
			var ls7 = new LineSegment(new Point(0, 0), new Point(0.5, 0));
			var ls8 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7, ls8 };

			var result1 = TraversalUtilities.FindLeftConnectedSegmentsSortedByAngle(ls3, segments);
			Assert.AreEqual(2, result1.Count);
			Assert.AreEqual(ls1, result1[0]);
			Assert.AreEqual(ls2, result1[1]);

			var result2 = TraversalUtilities.FindLeftConnectedSegmentsSortedByAngle(ls1, segments);
			Assert.AreEqual(0, result2.Count);
			
			var result3 = TraversalUtilities.FindLeftConnectedSegmentsSortedByAngle(ls4, segments);
			Assert.AreEqual(3, result3.Count);
			Assert.AreEqual(ls7, result3[0]);
			Assert.AreEqual(ls5, result3[1]);
			Assert.AreEqual(ls3, result3[2]);
		}

		[Test]
		public void TestFindRightConnectedSegmentsSortedByAngle()
		{
			//
			//    \       /
			//     \_____/___  ____
			//     |      \   
			//     |       \___
			//
			// -1.5 -1   0 0.5 1   2
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1.5, 1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(0.5, 1));
			var ls5 = new LineSegment(new Point(0, 0), new Point(0.5, -1));
			var ls6 = new LineSegment(new Point(0.5, -1), new Point(1, -1));
			var ls7 = new LineSegment(new Point(0, 0), new Point(0.5, 0));
			var ls8 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7, ls8 };

			var result1 = TraversalUtilities.FindRightConnectedSegmentsSortedByAngle(ls5, segments);
			Assert.AreEqual(1, result1.Count);
			Assert.AreEqual(ls6, result1[0]);

			var result2 = TraversalUtilities.FindRightConnectedSegmentsSortedByAngle(ls2, segments);
			Assert.AreEqual(0, result2.Count);

			var result3 = TraversalUtilities.FindRightConnectedSegmentsSortedByAngle(ls3, segments);
			Assert.AreEqual(3, result3.Count);
			Assert.AreEqual(ls4, result3[0]);
			Assert.AreEqual(ls7, result3[1]);
			Assert.AreEqual(ls5, result3[2]);
		}

		[Test]
		public void TestRemoveDanglingLineSegments_0()
		{
			var result = TraversalUtilities.RemoveDanglingLineSegments(new List<LineSegment>());
			Assert.AreEqual(0, result.Count);
		}

			[Test]
		public void TestRemoveDanglingLineSegments_1()
		{
			//
			//    \       /
			//     \_____/___  ____
			//     |      \   
			//     |       \___
			//
			// -1.5 -1   0 0.5 1   2
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1.5, 1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(0.5, 1));
			var ls5 = new LineSegment(new Point(0, 0), new Point(0.5, -1));
			var ls6 = new LineSegment(new Point(0.5, -1), new Point(1, -1));
			var ls7 = new LineSegment(new Point(0, 0), new Point(0.5, 0));
			var ls8 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7, ls8 };

			var result1 = TraversalUtilities.RemoveDanglingLineSegments(segments);
			Assert.AreEqual(0, result1.Count); 
		}

		[Test]
		public void TestRemoveDanglingLineSegments_2()
		{
			// _____________
			//     \       /
			//      \_____/___  ____
			//      |      \   
			//      |       \___
			//
			//-2-1.5-1    0 0.5 1   2
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1.5, 1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(0.5, 1));
			var ls5 = new LineSegment(new Point(0, 0), new Point(0.5, -1));
			var ls6 = new LineSegment(new Point(0.5, -1), new Point(1, -1));
			var ls7 = new LineSegment(new Point(0, 0), new Point(0.5, 0));
			var ls8 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var ls9 = new LineSegment(new Point(-2, 1), new Point(0.5, 1));
			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7, ls8, ls9 };

			var result1 = TraversalUtilities.RemoveDanglingLineSegments(segments);
			Assert.AreEqual(0, result1.Count); // only perfect loops may survive! 
		}

		[Test]
		public void TestRemoveDanglingLineSegments_3()
		{
			//     _________
			//     \       /
			//      \_____/___  ____
			//      |      \   
			//      |       \___
			//
			//-2-1.5-1    0 0.5 1   2
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1.5, 1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(0.5, 1));
			var ls5 = new LineSegment(new Point(0, 0), new Point(0.5, -1));
			var ls6 = new LineSegment(new Point(0.5, -1), new Point(1, -1));
			var ls7 = new LineSegment(new Point(0, 0), new Point(0.5, 0));
			var ls8 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var ls9 = new LineSegment(new Point(-1.5, 1), new Point(0.5, 1));
			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7, ls8, ls9 };

			var result1 = TraversalUtilities.RemoveDanglingLineSegments(segments);
			Assert.AreEqual(4, result1.Count);
			Assert.AreEqual(ls1, result1[0]);
			Assert.AreEqual(ls3, result1[1]);
			Assert.AreEqual(ls4, result1[2]);
			Assert.AreEqual(ls9, result1[3]);
		}
	}
}
