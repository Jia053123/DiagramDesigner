using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class SegmentsUtilitiesTests
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

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls2), Math.PI);
			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls2, ls1), Math.PI);

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls3), Math.PI * 1.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls3, ls1), Math.PI * 0.5);

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls4), Math.PI * 0.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls4, ls1), Math.PI * 1.5);

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls1), 0);
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
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1.5, -1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1.5, 1));
			var ls3 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(0.5, 1));
			var ls5 = new LineSegment(new Point(0, 0), new Point(0.5, -1));
			var ls6 = new LineSegment(new Point(0.5, -1), new Point(1, -1));
			var ls7 = new LineSegment(new Point(0, 0), new Point(0.5, 0));
			var ls8 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7, ls8 };

			var result1 = SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(ls3, segments);
			Assert.AreEqual(2, result1.Count);
			Assert.AreEqual(ls1, result1[0]);
			Assert.AreEqual(ls2, result1[1]);

			var result2 = SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(ls1, segments);
			Assert.AreEqual(0, result2.Count);
			
			var result3 = SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(ls4, segments);
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
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1.5, -1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(0.5, 1));
			var ls5 = new LineSegment(new Point(0, 0), new Point(0.5, -1));
			var ls6 = new LineSegment(new Point(0.5, -1), new Point(1, -1));
			var ls7 = new LineSegment(new Point(0, 0), new Point(0.5, 0));
			var ls8 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7, ls8 };

			var result1 = SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(ls5, segments);
			Assert.AreEqual(1, result1.Count);
			Assert.AreEqual(ls6, result1[0]);

			var result2 = SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(ls2, segments);
			Assert.AreEqual(0, result2.Count);

			var result3 = SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(ls3, segments);
			Assert.AreEqual(3, result3.Count);
			Assert.AreEqual(ls4, result3[0]);
			Assert.AreEqual(ls7, result3[1]);
			Assert.AreEqual(ls5, result3[2]);
		}
	}
}
